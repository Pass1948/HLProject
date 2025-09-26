using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnBasedManager : MonoBehaviour
{
    [HideInInspector] public TurnStateMachine turnHFSM { get; private set; }
    [HideInInspector] public TurnSettingValue turnSettingValue { get; private set; }


    public Queue<BaseEnemy> monsterQueue = new Queue<BaseEnemy>();

    // 현재 진행 중인 몬스터(없으면 null)
    private BaseEnemy currentEnemy;

    // 적 턴 진행 여부 플래그
    private bool enemyPhaseActive;

    public bool isStarted = false;

    private readonly Dictionary<Type, ITurnState> _stateCache = new Dictionary<Type, ITurnState>();
    private void Awake()
    {
        turnHFSM = new TurnStateMachine();
        gameObject.AddComponent<TurnSettingValue>();
        turnSettingValue = GetComponent<TurnSettingValue>();
    }
    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.PlayerAction, HandleActionSelection);
        // [추가] 적 턴 종료 이벤트 구독: 개별 적이 자신의 턴을 마치면 다음 적을 즉시 진행
        GameManager.Event.Subscribe(EventType.EnemyTurnEnd, OnEnemyTurnEnded);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.PlayerAction, HandleActionSelection);
        GameManager.Event.Unsubscribe(EventType.EnemyTurnEnd, OnEnemyTurnEnded);
    }

    private void Start()
    {
        // 초기 상태 설정
        SetTo<IdleState>();
        turnHFSM.selectedAction = PlayerActionType.None;
    }

    private void Update()
    {
        turnHFSM.Tick(Time.deltaTime);
        // [참고] 폴링 방식이 필요 없다면 이곳에서 UpdateMonster를 호출하지 않아도 됩니다.
    }
    private void FixedUpdate()
    {
        turnHFSM.FixedTick(Time.fixedDeltaTime);
    }
    
    public void StartTotalTurn() => isStarted = !isStarted;

    public void ChangeStartTurn()    // TODO:스테이지 시작과 종료 시점에 호출해주기 바람
    {
        StartTotalTurn();
        ChangeTo<IdleState>("Force");
    }
    
    public void SetTo<T>(string reason = null) where T : ITurnState, new()
    {
        var next = GetState<T>();
        turnHFSM.Set(next);
    }

    // 타 클래스에서 상태 전이 요청용
    public void ChangeTo<T>(string reason = null) where T : ITurnState, new()
    {
        var next = GetState<T>();
        turnHFSM.Change(next, reason);
    }

    // 상태 인스턴스 가져오기(없으면 생성 후 캐시에 등록)
    public T GetState<T>() where T : ITurnState, new()
    {
        var key = typeof(T);
        if (_stateCache.TryGetValue(key, out var s)) return (T)s;

        // 파라미터 없는 생성자로 생성
        var inst = new T();
        _stateCache[key] = inst;
        return inst;
    }

    // 필요 시 외부에서 초기화 리셋
    public void ResetAll()
    {
        _stateCache.Clear();
    }

    public string GetState()
    {
        var current = turnHFSM != null ? turnHFSM.Current : null;
        return current != null ? current.Name : "None";
    }

    private void HandleActionSelection()
    {
        switch (turnHFSM.selectedAction)
        {
            case PlayerActionType.Move:
                ChangeTo<PlayerMoveState>();
                turnSettingValue.actionSelected = false;
                break;
            case PlayerActionType.Attack:
                ChangeTo<PlayerAttackState>();
                turnSettingValue.actionSelected = false;
                break;
            case PlayerActionType.Kick:
                ChangeTo<PlayerKickState>();
                turnSettingValue.actionSelected = false;
                break;
        }
    }

    public void SetSelectedAction(PlayerActionType action)
    {
        turnHFSM.selectedAction = action;
        HandleActionSelection();
    }


    // ===== 적 턴 진행 순서로직 =====
    // 적 턴 시작 시 호출
    public void BeginEnemyPhase()
    {
        monsterQueue.Clear();
        currentEnemy = null;
        enemyPhaseActive = true;

        List<BaseEnemy> monsters = GameManager.Unit.enemies;
        foreach (var enemy in monsters)
        {
            if (enemy == null || enemy.controller == null) continue;
            if (enemy.controller.isDie) continue;
            enemy.controller.isDone = false;
            enemy.controller.startTurn = false;
            monsterQueue.Enqueue(enemy);
        }
        TryStartNextEnemy();
    }

    // 개별 적 턴 종료 시점에 호출되는 로직
    private void OnEnemyTurnEnded()
    {
        if (!enemyPhaseActive) return;
        if (currentEnemy != null && currentEnemy.controller != null)
        {
            // [중요] 완료 후 플래그 초기화
            currentEnemy.controller.startTurn = false;
            currentEnemy.controller.isDone = false;
            currentEnemy = null;
        }
        TryStartNextEnemy();

    }

    // 큐에서 다음 적을 하나 꺼내어 턴 시작
    private void TryStartNextEnemy()
    {
        while (monsterQueue.Count > 0)
        {
            var enemy = monsterQueue.Dequeue();
            if (enemy == null || enemy.controller == null) continue;
            if (enemy.controller.isDie) continue;

            if (!enemy.controller.startTurn)
            {
                enemy.controller.startTurn = true;
                enemy.controller.isDone = false;
                enemy.controller.StartTurn();
                currentEnemy = enemy;
                return; // 현재 적이 완료되면 OnEnemyTurnEnded 로 이어짐
            }
        }

        // 큐가 비었고 진행 중인 적이 없으면 적 턴 종료
        if (currentEnemy == null)
        {
            enemyPhaseActive = false;
            
            GameManager.Event.Publish(EventType.EnemyTurnEnd);
            
            ChangeTo<ClearCheckState>("Enemy phase finished");
        }
    }
    public bool EnemyDieCheck()
    {
        var monsters = (GameManager.Unit != null) ? GameManager.Unit.enemies : null;
        // 적 리스트가 없거나 비어 있으면 '모두 처치됨'으로 간주 (원하시면 false로 바꾸세요)
        if (monsters == null || monsters.Count == 0)
            return true;

        foreach (var e in monsters)
        {
            if (e == null)  // null 이면 바로 반환
                continue;

            // 하나라도 살아있으면 false 반환
            if (!e.controller.isDie)
                return false;
        }
        return true;
    }

    public bool IsPlayerDead()
    {
        var p = (GameManager.Unit != null) ? GameManager.Unit.Player : null;
        if (p == null) return true;

        bool flagDead = (p.controller != null) && p.playerModel.die;
        return flagDead;
    }
}
