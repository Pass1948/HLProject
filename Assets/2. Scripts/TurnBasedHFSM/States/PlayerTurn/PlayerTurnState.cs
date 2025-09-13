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
        // �� ���� �� Ŀ�ǵ� �ʱ�ȭ�� �Է� ���
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
