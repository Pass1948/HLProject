using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAction : MonoBehaviour
{
    public void ApplyDamage(Vector3Int myCellPos, int damageAmount)
    {
        BasePlayer player = FindPlayerByPosition(myCellPos);
        if (player != null)
        {
            GameManager.Unit.ChangeHealth(player.playerModel, damageAmount);
            return;
        }
        
        BaseEnemy enemy = FindEnemyByPosition(myCellPos);
        if (enemy != null)
        {
            GameManager.Unit.ChangeHealth(enemy.enemyModel, damageAmount);
        }
    }

    private BasePlayer FindPlayerByPosition(Vector3Int cellPos)
    {
        Vector3Int playerCellPos = GameManager.Map.tilemap.WorldToCell(GameManager.Unit.Player.transform.position);
        if (playerCellPos == cellPos)
        {
            return GameManager.Unit.Player;
        }
        return null;
    }

    private BaseEnemy FindEnemyByPosition(Vector3Int cellPos)
    {
        foreach (var enemy in GameManager.Unit.enemies)
        {
            if (enemy == null) continue;
            Vector3Int enemyCellPos = GameManager.Map.tilemap.WorldToCell(enemy.transform.position);
            if (enemyCellPos == cellPos)
            {
                return enemy;
            }
        }
        return null;
    }
}
