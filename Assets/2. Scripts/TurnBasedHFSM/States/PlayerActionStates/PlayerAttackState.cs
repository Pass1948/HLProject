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
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f) // TODO: Attack windup time
            {
                GameManager.UI.CloseUI<MainUI>();
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
            Debug.Log(GameManager.Unit.enemies[0].enemyModel);
            timer = turnSetVlaue.resetTime;

            //TODO: 첫 매개변수로 공격 -> 마우스 클릭시 감지되는 적 인덱스 넣어주기!! enemyModel로 해주시면 됩니다.
            GameManager.Unit.ChangeHealth(
                GameManager.Unit.enemies[0].enemyModel,
                GameManager.Unit.Player.playerModel.attack,
                turnSetVlaue.fireAmmo
                );
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f) // TODO: A_Execute time 
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
            if (timer > 0.1f) // TODO: A_Recover time 
            {
                GameManager.Map.pathfinding.ResetMapData();
                ChangeState<PlayerTurnEndState>();
            }
        }
    }
}
