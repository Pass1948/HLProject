using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerActionType
{
    None,
    Move,
    Attack,
    Kick
}
public class PlayerTurnState : BaseTurnState
{
    private PlayerActionType selectedAction = PlayerActionType.None;
    float timer;
    public PlayerTurnState() { }
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        // 턴 시작 시 커맨드 초기화후 입력 대기
        GameManager.Command.ClearAll();
    }

    public override void Tick(float dt)
    {
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime && turnSetVlaue.actionSelected==true)
        {
            HandleActionSelection();
        }
    }
    private void HandleActionSelection()
    {
        switch (selectedAction) 
        { 
            case PlayerActionType.Move:
                ChangeState<PlayerMoveState>("Move Selected");
                turnSetVlaue.actionSelected = false;
                break;
            case PlayerActionType.Attack:
                ChangeState<PlayerAttackState>("Attack Selected");
                turnSetVlaue.actionSelected = false;
                break;
            case PlayerActionType.Kick:
                ChangeState<PlayerKickState>("Kick Selected");
                turnSetVlaue.actionSelected = false;
                break;
        }
    }
}
