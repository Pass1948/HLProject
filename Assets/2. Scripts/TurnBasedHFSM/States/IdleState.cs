using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseTurnState
{
    public IdleState() { }
    public override void OnEnter()
    {
        ChangeState<PlayerTurnState>();
    }
    public override void Tick(float dt)
    {

    }
}
