using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemyState : BaseEnemyState
{
    public HitEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        animHandler.OnHit();
        // 데미지 적용시키기
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
