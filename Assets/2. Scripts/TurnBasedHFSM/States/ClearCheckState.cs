using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCheckState : BaseTurnState
{
    float timer;
    private bool didClose;
    public ClearCheckState() { }
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        didClose = false;
    }
    public override void Tick(float dt)
    {
        if (didClose) return;
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime)
        {
            if (turnManager.IsPlayerDead())
            {
                ChangeState<LoseState>();
            }
            else
            {
                ChangeState<IdleState>();
            }
        }
    }
}
