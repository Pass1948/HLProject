using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseTurnState
{
    float timer;
    public IdleState() { }
    
        
    public override void OnEnter() 
    {
        if (turnManager.isStarted == true)
        {
            timer =  turnSetVlaue.resetTime;
            GameManager.UI.OpenUI<PaseTurnUI>();
            Time.timeScale = 3f;    // 배속 기능
        }
    }
    public override void Tick(float dt)
    {
        if (turnManager.isStarted == true)
        {
            timer+= dt;
            if(timer>turnSetVlaue.turnDelayTime)
            {
                GameManager.UI.CloseUI<PaseTurnUI>();
                ChangeState<PlayerTurnState>();
            }
        }
    }
}
