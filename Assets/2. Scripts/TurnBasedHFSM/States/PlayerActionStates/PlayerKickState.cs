using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickState : PlayerActionState
{
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
            if (timer > 0.5f)
            {
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
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.5f)
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
            if (timer > 0.5f)
            {
                ChangeState<PlayerTurnEndState>();
            }
        }
    }
}
