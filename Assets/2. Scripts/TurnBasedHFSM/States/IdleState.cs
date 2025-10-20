using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseTurnState
{
    float timer;
    public IdleState() { }
    
    public override void OnEnter() 
    {
       Time.timeScale = 3f;    // 배속 기능
        if (turnManager.isStarted == true)
        {
            timer =  turnSetVlaue.resetTime;

        }
    }
    public override void Tick(float dt)
    {
        if (turnManager.isStarted == true)
        {
            timer+= dt;
            if(timer>0.5f)
            {
                GameManager.UI.OpenUI<FadeOutUI>();
                GameManager.UI.OpenUI<MainUI>();
                ChangeState<PlayerTurnState>();
            }
        }
    }
}
