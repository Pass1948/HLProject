using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickState : PlayerActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        ChangeState<K_Windup>();
        GameManager.Mouse.ToggleMovePhase();
        GameManager.Mouse.isMouse = false;
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
            ChangeAttirEnemy();
        }
        public override void Tick(float dt)
        {
            timer += dt;
            if (timer > 0.1f)
            {
                ChangeState<K_Recover>();
            }
        }
        void ChangeAttirEnemy()
        {
            var targets = GameManager.Map.CurrentEnemyTargets;
            if (targets != null && targets.Count > 0)
            {
                foreach (var enemy in targets)
                {
                    if (enemy == null || enemy.controller == null || enemy.controller.isDie) continue;
                    enemy.ChenageAttribute();
                    GameManager.UI.GetUI<EnemyInfoPopUpUI>().SetData(enemy.enemyModel.attri, enemy.enemyModel.rank,
                        enemy.enemyModel.attack, enemy.enemyModel.moveRange, enemy.enemyModel.currentHealth,
                        enemy.enemyModel.maxHealth);
                }
            }
        }
    }
    
    class K_Recover : BaseTurnState
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
            if (timer > 0.1f)
            {
                ChangeState<PlayerTurnEndState>();
            }
        }
    }
}
