using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoveInfo : MonoBehaviour
{
    // TODO:플레이어의 이동 정보 타일맵(장보석)
    // 1. 플레이어의 현재 위치 2. 플레이어의 이동 범위 3. 타일맵 4. 이동 정보 타일
    public void ShowMoveInfoRange(Vector3Int playerPos, int moveRange, Tilemap overlayTilemap, GameObject moveTile)
    {
        //overlayTilemap.ClearAllTiles();
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;
        int[,] mapData = GameManager.Map.mapData;
        

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int id = mapData[x, y];

                if(id != TileID.Terrain) continue;

                Vector3Int cell = new Vector3Int(x, y, 0);
                int distance = Mathf.Abs(playerPos.x - cell.x) + Mathf.Abs(playerPos.y - cell.y);
                if (distance <= moveRange)
                {
                    Vector3 worldPos = overlayTilemap.GetCellCenterWorld(cell);
                    var tileObj = Instantiate(moveTile, worldPos, Quaternion.identity);
                    tileObj.transform.SetParent(overlayTilemap.transform);
                }
            }
        }

    }
}
