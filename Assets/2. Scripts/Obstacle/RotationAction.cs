using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAction : MonoBehaviour
{
    private static readonly Vector3Int[] clockwiseOffsets = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, 1, 0)
    };
    
    public void RotateUnits(Vector3Int centerCellPos)
    {
        for (int i = 0; i < clockwiseOffsets.Length; i++)
        {
            Vector3Int checkCellPos = centerCellPos + clockwiseOffsets[i];
            
            if (GameManager.Map.IsInside(checkCellPos))
            {
                int nextIndex = (i - 1 + clockwiseOffsets.Length) % clockwiseOffsets.Length;
                Vector3Int nextCellPos = centerCellPos + clockwiseOffsets[nextIndex];
                
                // 비어있는지 확인
                if (GameManager.Map.IsMovable(nextCellPos))
                {
                    // 플레이어
                    BasePlayer playerToMove = FindPlayerByPosition(checkCellPos);
                    if (playerToMove != null)
                    {
                        // 타일 ID를 바닥으로
                        GameManager.Map.mapData[playerToMove.controller._cellPosition.x, playerToMove.controller._cellPosition.y] = (int)TileID.Terrain;
                        
                        // 새로운 위치로 이동
                        playerToMove.controller._cellPosition = nextCellPos;
                        playerToMove.controller.transform.position = GameManager.Map.tilemap.GetCellCenterWorld(nextCellPos);
                        
                        // 플레이어 타일 ID 업데이트
                        GameManager.Map.mapData[nextCellPos.x, nextCellPos.y] = (int)TileID.Player;
                    }
                    // 적 
                    BaseEnemy enemyToMove = FindEnemyByPosition(checkCellPos);
                    if (enemyToMove != null)
                    {
                        // 타일 ID를 바닥으로
                        GameManager.Map.mapData[enemyToMove.controller.GridPos.x, enemyToMove.controller.GridPos.y] = (int)TileID.Terrain;
                        
                        // 새로운 위치로 이동
                        enemyToMove.controller.GridPos = nextCellPos;
                        enemyToMove.controller.transform.position = GameManager.Map.tilemap.GetCellCenterWorld(nextCellPos);
                        
                        // 적 타일 ID 업데이트
                        GameManager.Map.mapData[nextCellPos.x, nextCellPos.y] = (int)TileID.Enemy;
                    }
                }
            }
        }
    }
    //플레이어 찾기
    private BasePlayer FindPlayerByPosition(Vector3Int cellPos)
    {
        Vector3Int playerCellPos = GameManager.Map.tilemap.WorldToCell(GameManager.Unit.Player.transform.position);
        if (playerCellPos == cellPos)
        {
            return GameManager.Unit.Player;
        }
        return null;
    }
    // 에너미 찾기
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
