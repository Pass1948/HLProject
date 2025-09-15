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
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 50% 확률로 벽 또는 바닥 생성
                if (Random.Range(0, 2) == 1)
                {
                    GameManager.Map.mapData[x, y] = TileID.Wall; 
                }
                else
                {
                    GameManager.Map.mapData[x, y] = TileID.Terrain;
                }
            }
        }

        // 맵 그리기
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                
                if (mapData[x, y] == TileID.Wall)
                {
                    if (wallTile != null)
                    {
                        tilemap.SetTile(tilePosition, wallTile);
                    }
                }
                else
                {
                    if (groundTile != null)
                    {
                        tilemap.SetTile(tilePosition, groundTile);
                    }
                }
            }
        }
    }
}
