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
        if (timer > turnSetVlaue.turnDelayTime && turnSetVlaue.actionSelected)
        {
            GameManager.UI.CloseUI<PaseTurnUI>();
                Debug.Log($"지금 상태는 이거닷!{turnHFSM.selectedAction}");
                HandleActionSelection();
        }
    }
    private void HandleActionSelection()
    {
        switch (turnHFSM.selectedAction) 
        { 
            case PlayerActionType.Move:
                ChangeState<PlayerMoveState>("Force");
                break;
            case PlayerActionType.Attack:
                ChangeState<PlayerAttackState>("Force");
                break;
            case PlayerActionType.Kick:
                ChangeState<PlayerKickState>("Force");
                break;
        }
        turnSetVlaue.actionSelected = false;
    }
}
