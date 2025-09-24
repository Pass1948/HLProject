using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DieEnemyState : BaseEnemyState
{
    public DieEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        if (controller.isDie)
        {
            animHandler.OnDie();
            // �ִϸ��̼� ���

            // �� �ı�
            Exit();
        }
    }

    public override void Excute()
    {
    }

    public override void Exit()
    {
        GameManager.Unit.enemies.Remove(controller.baseEnemy);
        controller.baseEnemy.gameObject.SetActive(false);
        //controller.baseEnemy.gameObject.Destroy();
    }
}
