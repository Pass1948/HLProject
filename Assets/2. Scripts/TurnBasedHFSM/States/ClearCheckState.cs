using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCheckState : BaseTurnState
{
   public ClearCheckState() { }
    public override void OnEnter()
    {
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
