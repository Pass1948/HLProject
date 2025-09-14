using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public int[,] mapData;
    public int mapWidth = 10;
    public int mapHeight = 10;

    public int moveRange;
    public Vector3Int playerPos;

    public MapCreator mapCreator;
    public SpawnPointPlayer playerSpawner;
    public SpawnPointObstacle obstacleSpawner;
    public SpawnPointMonster monsterSpawner;
    public PlayerMoveInfo playerMoveInfo;

    public Tilemap tilemap;

    private TileBase groundTile;
    private TileBase wallTile;
    private TileBase moveInfoTile;

    GameObject grid;

    void Awake()
    {
        mapCreator = gameObject.AddComponent<MapCreator>();
        playerSpawner = gameObject.AddComponent<SpawnPointPlayer>();
        obstacleSpawner = gameObject.AddComponent<SpawnPointObstacle>();
        monsterSpawner = gameObject.AddComponent<SpawnPointMonster>();
        playerMoveInfo = gameObject.AddComponent< PlayerMoveInfo >();
        
        groundTile = GameManager.Resource.Load<TileBase>(Path.Map + "White");
        wallTile = GameManager.Resource.Load<TileBase>(Path.Map + "Black");
        moveInfoTile = GameManager.Resource.Load<TileBase>(Path.Map + "MoveInfoTilemap");
        
    }
    
    void Start()
    {
        //TODO: Test 할 시 주석 풀어주세요잉 (장보석)
        grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");
        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        var moveInfo = GameManager.Resource.Create<GameObject>(Path.Map + "MoveInfoTilemap");
        tilemap = temp.GetComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);
        playerPos = GameManager.Data.playerData.playerMoveData.PlayerPos;

        mapData = new int[mapWidth, mapHeight];
        SpawnAll();
        mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);
        moveRange = GameManager.Data.playerData.playerMoveData.MoveRange;
        playerMoveInfo.ShowMoveInfoRange(playerPos, moveRange, tilemap, moveInfo);
    }

    public void CreateMap()
    {
        grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");
        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        tilemap = temp.GetComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);

        mapData = new int[mapWidth, mapHeight];
        SpawnAll();
        mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);
    }
    public void CreateMovePoint()
    {
        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        var movePointTilemap = temp.GetComponent<Tilemap>();
        movePointTilemap.transform.SetParent(grid.transform);
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
    // 해당 셀이 이동 가능한지 확인
    // Terrain 일때만 이동 가능
    public bool IsWalkable(Vector3Int cell)
    {
        // 지정된 맵의 범위 밖이면? 당연히 못가자나~
        if (cell.x < 0 || cell.x >= mapWidth || cell.y < 0 || cell.y >= mapHeight)
        {
            return false;
        }
        int id = mapData[cell.x, cell.y];
        return id == TileID.Terrain;
    }
}