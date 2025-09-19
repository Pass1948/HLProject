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
            GameManager.Event.Publish(EventType.PlayerAttack);
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f) // TODO: Attack windup time
            {
                GameManager.Map.attackRange.ClearAttackType();
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
            GameManager.UI.CloseUI<MainUI>();
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f) 
            {
              /*  // 범위내에 있는 적들 전원 공격
                var targets = GameManager.TurnBased.GetQueuedAttackTargets();
                if (targets != null && targets.Count > 0)
                {
                    foreach (var enemy in targets)
                    {
                        if (enemy == null || enemy.controller == null || enemy.controller.isDie) continue;

                        GameManager.Unit.ChangeHealth(
                            enemy.enemyModel,
                            GameManager.Unit.Player.playerModel.attack,
                            turnSetVlaue.fireAmmo
                        );
                        enemy.controller.OnHitState();
                    }
                }*/
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

            if(turnManager.monsterQueue.Count <= 0)
            {
                ChangeState<WinState>("Force");
            }
        }
        public override void Tick(float dt)
        {
            //ChangeState<ClearCheckState>(); <=몬스터 처리 후 클리어 체크
            timer += dt;
            if (timer > 3f) // TODO: A_Recover time 
            {
                GameManager.Map.pathfinding.ResetMapData();
                ChangeState<PlayerTurnEndState>();
            }
        }
    }
}
