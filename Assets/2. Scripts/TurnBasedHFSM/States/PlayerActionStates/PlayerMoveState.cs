using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        ChangeState<M_Windup>();
        GameManager.Mouse.ToggleMovePhase();
        GameManager.Mouse.OnSwitchIsClicked();
    }

    //이동 동작 나누기 : 선딜, 동작, 후딜
    class M_Windup : PlayerActionState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
            GameManager.Map.ClearPlayerRange();
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f)
            {
                ChangeState<M_Execute>();
            }
        }
    }
    // 데이터처리
    class M_Execute : PlayerActionState
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
                ChangeState<M_Recover>();
            }
        }
    }

    class M_Recover : PlayerActionState
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
                ChangeState<PlayerChooseState>();
            }
        }
    }
}
