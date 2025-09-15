using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{
    // 맵 생성
    public void GenerateMap(int[,] mapData, Tilemap tilemap, TileBase groundTile, TileBase wallTile)
    {
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;

        // 일단 무작위로 생성
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 0: 바닥 , 1: 벽, 2: 플레이어, 3:오토바이, 4:장애물, 5: 몬스터
                GameManager.Map.mapData[x, y] = Random.Range(0, 2); 
            }
        }
        // 크기에 맞게 그리기
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (mapData[x, y] == 1)
                {
                    tilemap.SetTile(tilePosition, wallTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, groundTile);
                }
            }
        }
    }
}
