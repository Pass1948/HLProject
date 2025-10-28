using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBossState : BaseBossState
{
    public EndBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        
        controller.ReduceCooldown();
        
        if (controller.isStun) controller.ReduceStunTurn();
        
        controller.CompleteTurn();
        
        controller.startTurn = false;
        controller.isDone = true;
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void Exit()
    {
    }

    public override void Excute()
    {
    }
}
