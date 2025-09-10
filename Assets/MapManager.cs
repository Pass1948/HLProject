using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public MapCreator mapCreator;
    public SpawnPointPlayer playerSpawner;
    //public SpawnPointObstacle obstacleSpawner;
    //public SpawnPointMonster monsterSpawner;
    
    public Tilemap tilemap;

    void Start()
    {
        // 맵 먼저 생성
        mapCreator.GenerateMap(tilemap);

        playerSpawner.SpawnPlayer(tilemap);
        //obstacleSpawner.SpawnObstacles(tilemap);
        //monsterSpawner.SpawnMonsters(tilemap);
    }
}