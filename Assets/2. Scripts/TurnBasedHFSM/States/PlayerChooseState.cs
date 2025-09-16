using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerActionType2
{
    None,
    Attack,
    Kick
}
public class PlayerChooseState : BaseTurnState
{
   public PlayerChooseState() { }
    float timer;
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        // 턴 시작 시 커맨드 초기화후 입력 대기
        GameManager.Event.Publish(EventType.PlayerMove);
    }

    public override void Tick(float dt)
    {
    }
    
}
