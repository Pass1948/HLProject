using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEnemyState : BaseEnemyState
{
    public DieEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

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
