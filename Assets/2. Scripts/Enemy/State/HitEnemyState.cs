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
    }

    public override void Exit()
    {
    }
}
