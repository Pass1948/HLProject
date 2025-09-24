using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRotatingEffect : MonoBehaviour, ITurnEndEffect
{
    // 3x3 위치
    private static readonly Vector3Int[] clockwiseOffsets = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),    // 위 0
        new Vector3Int(1, 1, 0),    // 우상 1
        new Vector3Int(1, 0, 0),    // 우 2
        new Vector3Int(1, -1, 0),   // 우하 3
        new Vector3Int(0, -1, 0),   // 하 4
        new Vector3Int(-1, -1, 0),  // 좌하 5
        new Vector3Int(-1, 0, 0),   // 좌 6
        new Vector3Int(-1, 1, 0)    // 좌상 7
    };

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
        Vector3Int centerCellPos = GameManager.Map.tilemap.WorldToCell(transform.position);

        // 주변 8칸을 순회하며 플레이어, 차량, 적이 있는지 확인
        for (int i = 0; i < clockwiseOffsets.Length; i++)
        {
            Vector3Int currentOffset = clockwiseOffsets[i];
            Vector3Int checkCellPos = centerCellPos + currentOffset;

            // 맵 경계 안에 있는지 확인
            if (GameManager.Map.IsInside(checkCellPos) &&
                GameManager.Map.mapData[checkCellPos.x, checkCellPos.y] != TileID.Terrain)
            {
                // 유닛의 다음 위치를 반시계 방향으로 계산
                // 현재 인덱스에서 1을 빼고 배열 길이를 더한 후 나머지 연산
                int nextIndex = (i - 1 + clockwiseOffsets.Length) % clockwiseOffsets.Length;
                Vector3Int nextOffset = clockwiseOffsets[nextIndex];
                Vector3Int nextCellPos = centerCellPos + nextOffset;

                if (GameManager.Map.IsMovable(nextCellPos))
                {
                    // 플레이어 이동
                    if (GameManager.Map.mapData[checkCellPos.x, checkCellPos.y] == TileID.Player)
                    {
                        //GameManager.Unit.Player.playerController.SetPosition(nextCellPos.x, nextCellPos.y);
                    }
                    // 오토바이 이동
                    else if (GameManager.Map.mapData[checkCellPos.x, checkCellPos.y] == TileID.Vehicle)
                    {
                        //GameManager.Unit.Vehicle.controller.SetPosition(nextCellPos.x, nextCellPos.y);
                    }
                    // 적 이동
                    else if (GameManager.Map.mapData[checkCellPos.x, checkCellPos.y] == TileID.Enemy)
                    {
                        BaseEnemy enemyToMove = FindEnemyByPosition(checkCellPos);
                        if (enemyToMove != null)
                        {
                            enemyToMove.controller.SetPosition(nextCellPos.x, nextCellPos.y);
                        }
                    }
                }
                else
                {
                    Debug.Log($"이동하려는 위치가 막혀있습니다.");
                }
            }
        }
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
