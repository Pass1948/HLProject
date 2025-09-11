using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEnemyState : BaseEnemyState
{
    public EndEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("End : Enter");
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
