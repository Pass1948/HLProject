using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBossState : BaseBossState
{
    public HitBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        animHandler.OnHit();
    }

    public override void Excute()
    {
        if(controller.model.currentHealth <=0)
        {
            controller.isDie = true;
            stateMachine.ChangeState(stateMachine.DieState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
    
    public override void Exit()
    {
    }

}
