using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    public static int[,] mapData;
    public int mapWidth = 10;
    public int mapHeight = 10;
    
    public MapCreator mapCreator;
    public SpawnPointPlayer playerSpawner;
    public SpawnPointObstacle obstacleSpawner;
    public SpawnPointMonster monsterSpawner;
    
    public Tilemap tilemap;
    public  Vector3Int playerPos;

    void Awake()
    {
        playerSpawner = gameObject.AddComponent<SpawnPointPlayer>();
        obstacleSpawner = gameObject.AddComponent<SpawnPointObstacle>();
        monsterSpawner = gameObject.AddComponent<SpawnPointMonster>();
        mapCreator = gameObject.AddComponent<MapCreator>();

        playerSpawner.playerPrefab = GameManager.Resource.Load<GameObject>(Path.Player + "Player");

    }
    
    void Start()
    {
        mapData = new int[mapWidth, mapHeight];
        // 맵 생성
        mapCreator.GenerateMap(tilemap);
        SpawnAll();
    }

    public void SpawnAll()
    {
        //플레이어 생성
        playerSpawner.SpawnPlayer(tilemap);

        //장애물 생성
        obstacleSpawner.SpawnObstacles(tilemap);

        //몬스터 생성
        monsterSpawner.SpawnMonsters(tilemap);
    }
}