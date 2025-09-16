using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 턴이 오기 전에 대기하고 있는 상태
public class IdleEnemyState : BaseEnemyState
{
    public IdleEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler)
    {
    }

    public bool StartTurn { get; set; } = false;

    public override void Enter()
    {
        //Debug.Log("Idle : Enter");

    }

    public override void Excute()
    {
        //Debug.Log("Idle : Excute");

        if (!StartTurn) return;

        stateMachine.ChangeState(stateMachine.EvaluateState); 

        // 대기중
        // do nothing!!
        // 턴 시작의 경우 Evaluate 전환
        // or
        // Hit 될 경우 Hit상태 전환
    }

    public override void Exit()
    {
        Debug.Log("Idle : Exit");
    }
}
