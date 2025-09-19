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
                Debug.Log($"공격댐");

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

            AttackEnemy();
            Debug.Log($"공격중");
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
                    Debug.Log($"지금 몬스터 : {enemy.enemyModel.unitName}, {enemy.enemyModel.currentHealth}");
                    GameManager.Unit.ChangeHealth(
                        enemy.enemyModel,
                        GameManager.Unit.Player.playerModel.attack,
                        turnSetVlaue.fireAmmo
                    );
                    Debug.Log($"지금 몬스터 : {enemy.enemyModel.unitName}, {enemy.enemyModel.currentHealth}");
                    enemy.controller.OnHitState();
                    Debug.Log($"지금 몬스터 : {enemy.enemyModel.unitName}, {enemy.enemyModel.currentHealth}");
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
                GameManager.Map.pathfinding.ResetMapData();
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
