using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public int[,] mapData;
    public int mapWidth = 10;
    public int mapHeight = 10;
    
    public MapCreator mapCreator;
    public SpawnPointPlayer playerSpawner;
    public SpawnPointObstacle obstacleSpawner;
    public SpawnPointMonster monsterSpawner;
    
    public Tilemap tilemap;

    private TileBase groundTile;
    private TileBase wallTile;

    GameObject grid;

    void Awake()
    {
        mapCreator = gameObject.AddComponent<MapCreator>();
        playerSpawner = gameObject.AddComponent<SpawnPointPlayer>();
        obstacleSpawner = gameObject.AddComponent<SpawnPointObstacle>();
        monsterSpawner = gameObject.AddComponent<SpawnPointMonster>();
        
        groundTile = GameManager.Resource.Load<TileBase>(Path.Map + "White");
        wallTile = GameManager.Resource.Load<TileBase>(Path.Map + "Black");
    }
    
    void Start()
    {
        grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");
        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        tilemap =  temp.GetComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);
        
        mapData = new int[mapWidth, mapHeight];
        SpawnAll();
        // 맵 생성
        mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);
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
    
    public void SetObjectPosition(int x, int y, int objectID)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            mapData[x, y] = objectID;
        }
    }

    // 이동할 때
    public void UpdateObjectPosition(int oldX, int oldY, int newX, int newY, int objectID)
    {
        // 이전 위치를 0으로
        if (oldX >= 0 && oldY >= 0 && oldX < mapWidth && oldY < mapHeight)
        {
            mapData[oldX, oldY] = 0;
        }

        // 새로운 위치에 기록
        if (newX >= 0 && newY >= 0 && newX < mapWidth && newY < mapHeight)
        {
            mapData[newX, newY] = objectID;
        }
    }
}