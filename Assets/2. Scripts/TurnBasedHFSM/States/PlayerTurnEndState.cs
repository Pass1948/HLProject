using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnEndState : BaseTurnState
{
    float timer;
    public PlayerTurnEndState() { }
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        GameManager.Data.playerData.playerMoveData.MoveRange = turnManager.savedMoveRange;
    }
    public override void Tick(float dt)
    {
        timer += dt;
        if (timer > 2f)// UI애니메이션 시간 으로 수정
        {
            ChangeState<EnemyTurnState>();
        }
    }
}
