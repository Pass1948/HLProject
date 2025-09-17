using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyState : BaseEnemyState
{
    public AttackEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("Attack : Enter");

        GameManager.Unit.ChangeHealth(GameManager.Unit.Player.playerModel, controller.model.attack);

        animHandler.OnAttack();

        stateMachine.ChangeState(stateMachine.EndState);
    }

    public override void Excute()
    {

        Debug.Log("Attack : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Attack : Exit");

    }
}
