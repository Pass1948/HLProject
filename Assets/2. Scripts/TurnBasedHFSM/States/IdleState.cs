using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseTurnState
{
    float timer;
    public IdleState() { }
    public override void OnEnter() 
    {
        timer =  turnSetVlaue.resetTime;
    }
    public override void Tick(float dt)
    {
        timer+= dt;
        if(timer>turnSetVlaue.turnDelayTime)
        {
            ChangeState<PlayerTurnState>();
        }
    }
}
