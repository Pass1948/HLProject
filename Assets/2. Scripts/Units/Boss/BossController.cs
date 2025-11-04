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
    public bool canNormalAttack;
    public bool isDie;
    public bool isDone;
    public bool startTurn = false;
    public bool isCooldown = false;  // 현재 쿨타임 중인가?
    public bool canPattern;          // 패턴 사용가능한가? -> 전 에너미 턴에 경고를 했는가
    public int cooldown;             // 쿨타임
    public int remainCooldown;       // 쿨타임을 연산할 변수
    public int patternPower;         // 패턴 공격력
    public int patternRange;         // 패턴 공격 범위
    public bool isStun;
    public int stunTurn;

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

    public void SetController(bool normalAttack, int cooldown, int patternPower, int patternRange)
    {
        this.canNormalAttack = normalAttack;
        this.cooldown = cooldown;
        this.remainCooldown = 0;
        this.patternPower = patternPower;
        this.patternRange = patternRange;
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
        GameManager.TurnBased.ChangeTo<PlayerTurnState>();
    }
    

    public IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("길이 비었음");
            yield break;
        }

        // foreach (var cell in path)
        // {
        //     Vector3 targetPos = GameManager.Map.tilemap.GetCellCenterWorld(cell);
        //     yield return StartCoroutine(MoveToPosition(targetPos, moveDuration));
        // }
        
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPos = GameManager.Map.tilemap.GetCellCenterWorld(path[i]);
            
            Vector3 dir = targetPos - transform.position;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                // animHandler.FaceToTarget4Dir(dir);
                yield return StartCoroutine(SmoothRotate(dir, 0.12f));
            }

            yield return StartCoroutine(MoveToPosition(targetPos, moveDuration));
        }
    }

    public IEnumerator SmoothRotate(Vector3 dir, float duration)
    {
        if (dir == Vector3.zero) yield break;

        Quaternion startRot = animHandler.modelTransform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -animHandler.rotationOffsetY, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            animHandler.modelTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        animHandler.modelTransform.rotation = targetRot;
    }
    
    public IEnumerator MoveToPosition(Vector3 target, float duration)
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


    public void PatternCooldown()
    {
        isCooldown = true;
        remainCooldown = cooldown + 1;
    }

    public void ReduceCooldown()
    {
        if (isCooldown)
        {
            remainCooldown--;
            if (remainCooldown <= 0)
            {
                isCooldown = false;
                Debug.Log("패턴 쿨타임 해제됨");
            }
        }
    }

    public void StunStart()
    {
        isStun = true;
        stunTurn = 2;
        canPattern = false;
    }

    public void ReduceStunTurn()
    {
        if (isStun)
        {
            stunTurn--;
            if (stunTurn <= 0)
            {
                isStun = false;
            }
        }
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
