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
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                
                // 맵의 바깥 // X가 -1이거나 mapWidth, Y가 -1이거나 mapHeight일 경우 
                if (x == -1 || x == mapWidth || y == -1 || y == mapHeight)
                {
                    // 경계 TileID.Obstacle
                    GameManager.Map.mapData[x, y] = TileID.Obstacle;
                    tilemap.SetTile(tilePosition, groundTile);
                }
                else
                {
                    // 내부
                    GameManager.Map.mapData[x, y] = TileID.Terrain;
                    tilemap.SetTile(tilePosition, groundTile);
                }

            }
        }
    }
}