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
            Debug.Log("이런 싯팔이거 왜 실행 안대 개시발ClearCheckState Tick : " + turnManager.IsPlayerDead());
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
