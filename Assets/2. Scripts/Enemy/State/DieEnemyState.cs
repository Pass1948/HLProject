using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEnemyState : BaseEnemyState
{
    public DieEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("Die : Enter");

        controller.isDie = true;

        if (controller.isDie)
        {
            animHandler.OnDie();
           // 애니메이션 재생
           // 후 파괴

           
        }
    }

    public override void Excute()
    {
        Debug.Log("Die : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Die : Exit");
    }
}
