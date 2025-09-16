using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyModel model;
    public EnemyAnimHandler animHandler;
    private EnemyStateMachine stateMachine;
    
    public Vector3Int GridPos { get; set; }
    public Vector3Int TargetPos { get; set; }

    public int moveRange;
    public int minAttackRange;
    public int maxAttackRange;
    public bool isDie;
    public bool isDone = false;
    public bool startTurn =false;

    public float moveDuration = 0.2f;

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.EnemyTurnStart, StartTurn);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.EnemyTurnStart, StartTurn);
    }

    public void InitController()
    {
        Vector2Int player = GameManager.Map.GetPlayerPosition();
        // 상태머신 할당, Init 초기 상태 Idle로

        moveRange = model.moveRange;
        minAttackRange = model.minAttackRange;
        maxAttackRange = model.maxAttackRange;
        isDie = model.isDie;
        Debug.Log(moveRange);

        RunStateMaching();
    }

    public void RunStateMaching()
    {
        stateMachine = new EnemyStateMachine(animHandler, this);
        if (stateMachine == null) Debug.Log("상태머신 없슴");
        stateMachine.Init();
    }

    public void SetPosition(int x, int y)
    {
        GridPos = new Vector3Int(x, y, 0);
        //Debug.Log($"Enemy{GridPos}");
    }

    public void UpdatePlayerPos()
    {
        Vector2Int player = GameManager.Map.GetPlayerPosition();

        if (player.x != -1) // 찾았으면
        {
            TargetPos = new Vector3Int(player.x, player.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine?.Excute();

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(TargetPos);

            StartTurn();
            //stateMachine.ChangeState(stateMachine.EvaluateState);
            //StartTurn(GameManager.Map.playerPos);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            OnHitState();
        }
    }




    private void OnHitState()
    {
        stateMachine.ChangeState(stateMachine.HitState);
    }

    public void StartTurn()
    {
        // 이미 죽었으면 무시
        if (isDie) return;

        // 중복 시작 방지
        if (startTurn && !isDone) return;

        // 턴 시작 시점에 플래그 초기화(중앙집중)
        isDone = false;      
        startTurn = true;
        stateMachine.ChangeState(stateMachine.EvaluateState);
    }

    //  적의 행동이 완전히 끝나는 시점(예: 상태머신의 마지막 상태 OnExit 등)에서 호출하세요.[추가:이영신]
    public void CompleteTurn()
    {
        if (isDie) return;
        // 완료 플래그 설정
        isDone = true;
        // 이벤트 발행: TurnBasedManager가 이 신호를 받아 다음 적을 진행
        GameManager.Event.Publish(EventType.EnemyTurnEnd);
    }

    public IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("길이 비었음");
            yield break;
        }


        foreach (var cell in path)
        {
            Vector3 targetPos = GameManager.Map.tilemap.GetCellCenterWorld(cell);
            yield return StartCoroutine(MoveToPosition(targetPos, moveDuration));
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

}
