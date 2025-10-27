using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickState : PlayerActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        ChangeState<K_Windup>();
        GameManager.Mouse.isMouse = false;
        GameManager.Mouse.isShowRange = false;
    }

    // 킥 동작 나누기 : 선딜, 동작, 후딜
    class K_Windup : BaseTurnState
    {
        float timer;
        public override void OnEnter()
        {
            GameManager.Unit.Player.animHandler.PlayerKickAnim(GameManager.Mouse.pointer);
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
            GameManager.Event.Publish(EventType.EnemyUIUpdate);
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
            if (targets != null && targets.Count <= 0)
            {
                if (GameManager.Unit.boss == null) 
                    return;
                GameManager.Unit.boss.ChenageAttribute();
                    GameManager.UI.GetUI<BossInfoPopUpUI>().SetData(
                        GameManager.Unit.boss.model.attri,
                        GameManager.Unit.boss.model.rank, 
                        GameManager.Unit.boss.model.attack,
                        GameManager.Unit.boss.model.moveRange, 
                        GameManager.Unit.boss.model.currentHealth,
                        GameManager.Unit.boss.model.maxHealth);
                    return;
            }
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
