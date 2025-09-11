using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemyState : BaseEnemyState
{
    public HitEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Hit : Enter");
    }

    public override void Excute()
    {
        Debug.Log("Hit : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Hit : Exit");
    }
}
