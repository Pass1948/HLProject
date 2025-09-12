using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEnemyState : BaseEnemyState
{
    public DieEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("Die : Enter");
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
