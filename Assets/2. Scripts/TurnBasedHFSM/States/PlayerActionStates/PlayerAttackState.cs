using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        ChangeState<A_Windup>();
    }

    // 공격 동작 나누기 : 선딜, 동작, 후딜
    class A_Windup : PlayerActionState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
            GameManager.Event.Publish(EventType.PlayerMove);
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
    class A_Execute : PlayerActionState
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

    class A_Recover : PlayerActionState
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
