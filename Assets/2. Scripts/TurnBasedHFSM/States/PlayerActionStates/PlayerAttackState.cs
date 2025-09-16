using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerActionState
{
    // 공격 동작 나누기 : 선딜, 동작, 후딜
    class A_Windup : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
            Debug.Log("Attack Windup");
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.5f) // TODO: Attack windup time
            {
                ChangeState<A_Execute>();
            }
        }
    }
    // 데이터처리
    class A_Execute : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.5f) // TODO: A_Execute time 
            {
                ChangeState<A_Recover>();
            }
        }
    }

    class A_Recover : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
        }
        public override void Tick(float dt)
        {
            //ChangeState<ClearCheckState>(); <=몬스터 처리 후 클리어 체크
            timer += dt;
            if (timer > 0.5f) // TODO: A_Recover time 
            {
                ChangeState<PlayerTurnEndState>();
            }
        }
    }
}
