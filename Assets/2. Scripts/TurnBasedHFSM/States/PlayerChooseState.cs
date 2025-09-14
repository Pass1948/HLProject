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
    private PlayerActionType2 selectedAction = PlayerActionType2.None;
    float timer;
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        // 턴 시작 시 커맨드 초기화후 입력 대기
    }

    public override void Tick(float dt)
    {
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime && turnSetVlaue.actionSelected == true)
        {
            HandleActionSelection();
        }
    }
    private void HandleActionSelection()
    {
        switch (selectedAction)
        {
            case PlayerActionType2.Attack:
                ChangeState<PlayerAttackState>("Attack Selected");
                turnSetVlaue.actionSelected = false;
                break;
            case PlayerActionType2.Kick:
                ChangeState<PlayerKickState>("Kick Selected");
                turnSetVlaue.actionSelected = false;
                break;
        }
    }
}
