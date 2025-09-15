using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    private Pathfinding pathfinding;

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
        //pathfinding = new Pathfinding(tilemap);
        ////

        //grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");
        //var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        //tilemap = temp.GetComponent<Tilemap>();
        //tilemap.transform.SetParent(grid.transform);

        //mapData = new int[mapWidth, mapHeight];
        //// 맵 생성
        //mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);
        //SpawnAll();
    }

    public void CreateMap()
    {
        pathfinding = new Pathfinding(tilemap);
        grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");
        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        tilemap = temp.GetComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);

        mapData = new int[mapWidth, mapHeight];
        mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);
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
            mapData[oldX, oldY] = TileID.Terrain;
        }

        // 새로운 위치에 기록
        if (newX >= 0 && newY >= 0 && newX < mapWidth && newY < mapHeight)
        {
            mapData[newX, newY] = objectID;
        }
    }

    // 임시로 선언한 함수들

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int dest)
    {
        return pathfinding.FindPath(start, dest);
    }

    public Vector2Int GetPlayerPosition()
    {

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                if (mapData[x, y] == TileID.Player)
                    return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool IsMovable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) return false;

        return mapData[x, y] == TileID.Terrain;
    }

    public bool IsPlayer(int x, int y)
    {
        return mapData[x, y] == TileID.Player;
    }

    public bool IsObstacle(int x, int y)
    {
        return mapData[x, y] == TileID.Obstacle;
    }

    public bool IsEnemy(int x, int y)
    {
        return mapData[x, y] == TileID.Enemy;
    }
}