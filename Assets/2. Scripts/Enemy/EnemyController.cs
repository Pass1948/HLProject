using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyStateMachine stateMachine;
    private EnemyAnimHandler animHandler;
    
    public Vector3Int GridPos { get; set; }
    public Vector3Int TargetPos { get; set; }
    public int MoveRange = 2;
    public int AttackRange = 1;
    public int HP = 30;


    private void Awake()
    {
        animHandler = GetComponent<EnemyAnimHandler>();

        // 상태머신 할당, Init 초기 상태 Idle로
        stateMachine = new EnemyStateMachine(animHandler, this);
        stateMachine.Init();
    }

    private void Start()
    {
        Debug.Log(GridPos);
        stateMachine.Init();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Excute();

        Debug.Log(TargetPos); 
        if (Input.GetKeyDown(KeyCode.A))
        {
            //StartTurn();
            //stateMachine.ChangeState(stateMachine.EvaluateState);
            //StartTurn(GameManager.Map.playerPos);
        }
    }

    public void StartTurn(Vector3Int playerPos)
    {
        TargetPos = playerPos;
        stateMachine.IdleState.StartTurn = true;
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateMachine.ChangeState(stateMachine.EvaluateState);
        }
    }
}
