using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{
    public TileBase groundTile;
    public TileBase wallTile;

    // 맵 생성
    public void GenerateMap(Tilemap tilemap)
    {
        // 맵 크기
        int mapWidth = 10;
        int mapHeight = 10;
        int[,] mapData = new int[mapWidth, mapHeight];

        // 일단 무작위로 생성
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                mapData[x, y] = Random.Range(0, 2); // 0 또는 1
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
