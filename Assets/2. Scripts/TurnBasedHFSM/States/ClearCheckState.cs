using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCheckState : BaseTurnState
{
    float timer;
    public ClearCheckState() { }
    public override void OnEnter()
    {
        timer = 0f;
    }
    public override void Tick(float dt)
    {
         if (turnManager.EnemyDieCheck())
        {
            ChangeState<WinState>();
        }
       else if (turnManager.IsPlayerDead())
        {
            ChangeState<LoseState>();
        }
        else
        {
            ChangeState<IdleState>();
        }
    }
}
