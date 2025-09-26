using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class StrategyManager : MonoBehaviour
{
    public Ammo fireAmmo;

    public void AttackEnemy()
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
                    fireAmmo
                );
                enemy.controller.OnHitState();
            }
        }
    }
    
    
    
}