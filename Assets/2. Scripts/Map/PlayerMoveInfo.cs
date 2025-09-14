using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoveInfo : MonoBehaviour
{
    // TODO:플레이어의 이동 정보 타일맵(장보석)
    // 1. 플레이어의 현재 위치 2. 플레이어의 이동 범위 3. 타일맵 4. 이동 정보 타일
    public void ShowMoveInfoRange(Vector3Int playerPos, int moveRange, Tilemap overlayTilemap)
    {
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;
        int[,] mapData = GameManager.Map.mapData;

        for (int x = -moveRange; x <= moveRange; x++)
        {
            for (int y = -moveRange; y <= moveRange; y++)
            {
                Vector3Int cell = new Vector3Int(playerPos.x, playerPos.y, playerPos.z);

                if (cell.x < 0 || cell.x >= mapWidth || cell.y < 0 || cell.y >= mapHeight) continue;

                int id = mapData[cell.x, cell.y];
                if(id != TileID.Terrain) continue;

                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                if (distance <= moveRange)
                {
                    Debug.Log($"PlayerMoveInfo - ShowMoveInfoRange: cell={cell}, distance={distance}");
                    Vector3 worldPos = overlayTilemap.GetCellCenterWorld(cell);
                    var tileObj = GameManager.Resource.Create<GameObject>(Path.Map + "MoveInfoTilemap");
                    tileObj.transform.SetParent(overlayTilemap.transform);
                    tileObj.transform.position = worldPos;
                }
            }
        }
    }
}
