using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public BaseBoss baseBoss;
    public BossModel model;
    public EnemyAnimHandler animHandler;
    public BossStateMachine stateMachine;

    public Vector3Int GridPos { get; set; }
    public Vector3Int TargetPos { get; set; }

    public int moveRange;
    public int minAttackRange;
    public int maxAttackRange;
    public bool isDie;
    public bool isDone;
    public bool startTurn = false;
    public bool isCooldown = false;
    public bool canPattern;
    public int cooldown;
    public int patternPower;

    public float moveDuration = 0.2f;

    // 골렘 패턴 공격범위 리스트
    public List<Vector3Int> warningCells = new List<Vector3Int>();
    
    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.EnemyTurnStart, StartTurn);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.EnemyTurnStart, StartTurn);
    }

    public void InitController(BaseBoss boss)
    {
        baseBoss = boss;
        moveRange = model.moveRange;
        minAttackRange = model.minAttackRange;
        maxAttackRange = model.maxAttackRange;
        isDie = model.isDie;

        RunStateMachine();
    }

    public void SetController(int cooldown, int patternPower)
    {
        this.cooldown = cooldown;
        this.patternPower = patternPower;
    }

    private void RunStateMachine()
    {
        stateMachine = new BossStateMachine(this, animHandler);
        if (stateMachine == null) Debug.LogError("보스 스테이트 머신 없슴");
        stateMachine.Init();
    }

    public void SetPosition(int x, int y)
    {
        GridPos = new Vector3Int(x, y, 0);
    }
    
    public void UpdatePlayerPos()
    {
        Vector2Int player = GameManager.Map.GetPlayer2Position();

        if (player.x != -1) // 찾았으면
        {
            TargetPos = new Vector3Int(player.x, player.y, 0);
        }
    }
    
    private void StartTurn()
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

    private void Update()
    {
        stateMachine?.Excute();
    }
    
    public void OnHitState()
    {
        stateMachine.ChangeState(stateMachine.HitState);
    }
    
    public void CompleteTurn()
    {
        if (isDie) return;
        // 완료 플래그 설정
        isDone = true;
        GameManager.Map.pathfinding.ResetMapData();
        GameManager.Event.Publish(EventType.EnemyTurnEnd);        // 이벤트 발행: TurnBasedManager가 이 신호를 받아 다음 적을 진행
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

    public void OnClickPreviewPath()
    {
        Vector3Int playerPos = TargetPos;
        Vector3Int enemyPos = GridPos;

        List<Vector3Int> path = GameManager.Map.FindPath(enemyPos, playerPos);

        if (path == null || path.Count == 0)
        {
            Debug.Log("노 이동");
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        if (path[path.Count - 1] == playerPos && GameManager.Map.IsPlayer(playerPos))
        {
            path.RemoveAt(path.Count - 1);
        }

        GameManager.Map.PlayerUpdateRange(GridPos, moveRange);
    }
    
    public void ReleasePreviewPath()
    {
        GameManager.Map.ClearPlayerRange();
    }

}
