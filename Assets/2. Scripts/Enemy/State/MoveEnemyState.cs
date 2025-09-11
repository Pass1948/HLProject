using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemyState : BaseEnemyState
{
    public MoveEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Move : Enter");
    }

    public override void Excute()
    {
        Debug.Log("Move : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Move : Exit");

    }
}
