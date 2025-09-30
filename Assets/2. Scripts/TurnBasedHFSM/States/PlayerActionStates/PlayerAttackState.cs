using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        ChangeState<A_Windup>();
        GameManager.Mouse.isMouse = false;
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
            if (timer > 0.1f) 
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
            AttackEnemy();
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f) 
            {
                ChangeState<A_Recover>();
            }
        }

        void AttackEnemy()
        {
            // 범위내에 있는 적들 전원 공격
            var targets = GameManager.Map.CurrentEnemyTargets;
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
            }
        }


    }

    class A_Recover : PlayerActionState
    {
        float timer;
        public override void OnEnter()
        {
            timer = turnSetVlaue.resetTime;
            GameManager.Map.attackRange.ClearAttackType();
        }
        public override void Tick(float dt)
        {

            timer += dt;
            if (timer > 1f) 
            {
                if (turnManager.EnemyDieCheck())
                {
                    ChangeState<WinState>();
                }
                else
                {
                    ChangeState<PlayerTurnEndState>();
                }
            }
        }
    }
}
