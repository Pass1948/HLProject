using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickState : PlayerActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        ChangeState<K_Windup>();
    }

    // 킥 동작 나누기 : 선딜, 동작, 후딜
    class K_Windup : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f)
            {
                GameManager.UI.CloseUI<MainUI>();
                ChangeState<K_Execute>();
            }
        }
    }
    // 데이터처리
    class K_Execute : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
            // TODO : 여기서 발차기 했을 때 정보 바꿔주기
            GameManager.Unit.enemies[0].ChenageAttribute();
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f)
            {
                ChangeState<K_Recover>();
            }
        }
    }
    
    class K_Recover : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f)
            {
                GameManager.Map.pathfinding.ResetMapData();
                ChangeState<PlayerTurnEndState>();
            }
        }
    }
}
