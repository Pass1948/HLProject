using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyModel model;
    private EnemyStateMachine stateMachine;
    private EnemyAnimHandler animHandler;
    
    public Vector3Int GridPos { get; set; }
    public Vector3Int TargetPos { get; set; }
    public int MoveRange;
    public int AttackRange;
    public int isDie;
    public int isDone;
    public float moveDuration = 0.2f;

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.CommandBuffered, StartTurn);
    }

    private void Awake()
    {
        
        
    }

    private void Start()
    {
        stateMachine.Init();
        Vector2Int player = GameManager.Map.GetPlayerPosition();
    }

    public void Init(EnemyModel model, EnemyAnimHandler animHandle)
    {
        // 상태머신 할당, Init 초기 상태 Idle로
        animHandler = animHandle;
        this.model = model;
        stateMachine = new EnemyStateMachine(animHandler, this);
    }

    public void SetPosition(int x, int y)
    {
        GridPos = new Vector3Int(x, y, 0);
        Debug.Log($"Enemy{GridPos}");
    }

    public void InitTarget()
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
        stateMachine.Excute();

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


    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.CommandBuffered, StartTurn);
    }

    private void OnHitState()
    {
        stateMachine.ChangeState(stateMachine.HitState);
    }

    public void StartTurn()
    {
        
        stateMachine.IdleState.StartTurn = true;
        stateMachine.ChangeState(stateMachine.EvaluateState);

        // 각각의 에너미의 StarTurn
    }

    public IEnumerator MoveAlongPath(List<Vector3Int> path)
    {
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
