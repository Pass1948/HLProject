using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamageEffect : MonoBehaviour, ITurnEndEffect
{
    [SerializeField] private int damageAmount = 1;

    private void OnEnable()
    {
        GameManager.Map.RegisterTurnEndEffect(this);
    }
    
    private void OnDisable()
    {
        if (GameManager.Map != null)
        {
            GameManager.Map.UnregisterTurnEndEffect(this);
        }
    }

    public void OnTurnEndAction()
    {
        Vector3Int myCellPos = GameManager.Map.tilemap.WorldToCell(transform.position);
        
        if (!GameManager.Map.IsInside(myCellPos))
        {
            return;
        }

        // 좌표에 있는 타일 ID
        int tileID = GameManager.Map.mapData[myCellPos.x, myCellPos.y];
        
        // 타일 ID를 통해 대상이 플레이어, 적, 모터 판단
        if (tileID == TileID.Player)
        {
            // 플레이어
            if (GameManager.Unit.Player != null)
            {
                GameManager.Unit.ChangeHealth(GameManager.Unit.Player.playerModel, damageAmount);
            }
        }
        // 적
        else if (tileID == TileID.Enemy)
        {
            BaseEnemy targetEnemy = FindEnemyByPosition(myCellPos);
            if (targetEnemy != null)
            {
                GameManager.Unit.ChangeHealth(targetEnemy.enemyModel, damageAmount);
            }
        }
        else if (tileID == TileID.Motor)
        {
            if (GameManager.Unit.Vehicle != null)
            {
                GameManager.Unit.ChangeHealth(GameManager.Unit.Vehicle.vehicleModel, damageAmount);
            }
        }

    }
    // 해당 위치의 적을 찾기
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
