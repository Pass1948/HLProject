using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTurnState : BaseTurnState
{
    float timer;
    public PlayerTurnState() { }
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        // 턴 시작 시 커맨드 초기화후 입력 대기
        GameManager.UI.OpenUI<PaseTurnUI>();
        
    }

    public override void Tick(float dt)
    {
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime )
        {
            GameManager.UI.CloseUI<PaseTurnUI>();
            GameManager.Event.Publish(EventType.PlayerMove);
        }
    }
}
