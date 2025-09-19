using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VHierarchy.Libs;

public class DieEnemyState : BaseEnemyState
{
    public DieEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {

        controller.isDie = true;

        if (controller.isDie)
        {
            animHandler.OnDie();
            // 애니메이션 재생

            // 후 파괴
            Exit();
        }
    }

    public override void Excute()
    {
    }

    public override void Exit()
    {
        GameManager.Unit.enemies.Remove(this.stateMachine.Controller.baseEnemy);
        controller.baseEnemy.gameObject.Destroy();
    }
}
