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

    public int moveRange = 10;
    public Vector3Int playerPos;

    public MapCreator mapCreator;
    public SpawnPointPlayer playerSpawner;
    public SpawnPointObstacle obstacleSpawner;
    public SpawnPointMonster monsterSpawner;
    public PlayerMoveInfo playerMoveInfo;

    private Pathfinding pathfinding;

    public Tilemap tilemap;
    public Tilemap moveInfoTilemap;

    private TileBase groundTile;
    private TileBase wallTile;
    private TileBase moveInfoTile;
    public int playerPosRange;

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
        moveInfoTile = GameManager.Resource.Load<TileBase>(Path.Map + "Green");

    }
    void Start()
    {
        //TODO: Test 할 시 주석 풀어주세요잉 (장보석)
        grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");

        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        tilemap = temp.GetComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);

        var moveInfo = GameManager.Resource.Create<GameObject>(Path.Map + "moveInfoTilemap");
        moveInfoTilemap = moveInfo.GetComponent<Tilemap>();
        moveInfoTilemap.transform.SetParent(grid.transform);

        mapData = new int[mapWidth, mapHeight];
        SpawnAll();
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    playerPos = GameManager.Data.playerData.playerMoveData.PlayerPos;
        //    moveRange = GameManager.Data.playerData.playerMoveData.MoveRange;
        //    Debug.Log($"{playerPos}");
        //    PlayerUpdateRange();
        //}
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

    // 플레이어 이동 범위 업데이트
    public void PlayerUpdateRange()
    {
        playerMoveInfo.ShowMoveInfoRange(playerPos, moveRange, moveInfoTile, moveInfoTilemap);
    }
    public void ClearPlayerRange()
    {
        playerMoveInfo.RemoveMoveInfoRange(moveInfoTilemap);
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

    public bool IsMovable(Vector3Int cell)
    {
        if (cell.x < 0 || cell.y < 0 || cell.x >= mapWidth || cell.y >= mapHeight) return false;

        int tileID = mapData[cell.x, cell.y];
        return tileID == TileID.Terrain;
    }

    public bool IsPlayer(Vector3Int cell)
    {
        return mapData[cell.x, cell.y] == TileID.Player;
    }

    public bool IsObstacle(Vector3Int cell)
    {
        return mapData[cell.x, cell.y] == TileID.Obstacle;
    }

    public bool IsEnemy(Vector3Int cell)
    {
        return mapData[cell.x, cell.y] == TileID.Enemy;
    }

}