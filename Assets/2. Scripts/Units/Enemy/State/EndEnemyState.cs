using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEnemyState : BaseEnemyState
{
    public EndEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        controller.CompleteTurn();
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void Excute()
    {
    }

    public override void Exit()
    {
    }
}
