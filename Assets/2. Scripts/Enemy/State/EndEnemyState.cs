using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEnemyState : BaseEnemyState
{
    public EndEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("End : Enter");

        controller.isDone = true;
    }

    public override void Excute()
    {
        Debug.Log("End : Excute");
    }

    public override void Exit()
    {
        Debug.Log("End : Exit");
    }


}
