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
    
    public int moveRange = 3;
    public Vector3Int playerPos;
    public int playerPosRange;
    
    public MapCreator mapCreator;
    public PlayerMoveInfo playerMoveInfo;
    private SpawnController spawnController;
    public AttackRangeDisplay attackRange;
        
    public Pathfinding pathfinding;
    public Tilemap tilemap;
    public Tilemap moveInfoTilemap;
    public Tilemap attackRangeTilemap;

    private TileBase groundTile;
    private TileBase wallTile;
    private TileBase moveInfoTile;
    private TileBase redAttackTile;

    public Grid grid;

    public List<BaseEnemy> CurrentEnemyTargets { get; private set; } = new List<BaseEnemy>();
    // 범위에 들어온 파괴 가능한 장애물 리스트
    public List<Vector3Int> CurrentObstacleCoords { get; private set; } = new List<Vector3Int>();
    // 턴끝 행동을 위한 장애물 리스트
    public List<ITurnEndEffect> turnEndEffects = new List<ITurnEndEffect>();

    void Awake()
    {
        mapCreator = gameObject.AddComponent<MapCreator>();
        spawnController = gameObject.AddComponent<SpawnController>();
        playerMoveInfo = gameObject.AddComponent< PlayerMoveInfo >();
        attackRange = gameObject.AddComponent<AttackRangeDisplay>();
        
        spawnController.InitializeSpawnersAndPools();
        
        groundTile = GameManager.Resource.Load<TileBase>(Path.Map + "White");
        wallTile = GameManager.Resource.Load<TileBase>(Path.Map + "Black");
        moveInfoTile = GameManager.Resource.Load<TileBase>(Path.Map + "Green");
        redAttackTile = GameManager.Resource.Load<TileBase>(Path.Map + "RedAttackTile");

    }
    void Start()
    {
        //TODO: Test 할 시 주석 풀어주세요잉 (장보석)
        //grid = GameManager.Resource.Create<GameObject>(Path.Map + "Grid");

        //var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        //tilemap = temp.GetComponent<Tilemap>();
        //tilemap.transform.SetParent(grid.transform);

        //var moveInfo = GameManager.Resource.Create<GameObject>(Path.Map + "MoveInfoTilemap");
        //moveInfoTilemap = moveInfo.GetComponent<Tilemap>(); 
        //moveInfoTilemap.transform.SetParent(grid.transform);

        //mapData = new int[mapWidth, mapHeight];
        //mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);

        //spawnController.SpawnAllObjects(); // SpawnAll();에서 변경
    }

    public void CreateMap()
    {
        Camera mainCamera = Camera.main;
        grid = GameManager.Resource.Create<Grid>(Path.Map + "Grid");

        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        tilemap = temp.GetComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);

        var moveInfo = GameManager.Resource.Create<GameObject>(Path.Map + "MoveInfoTilemap");
        moveInfoTilemap = moveInfo.GetComponent<Tilemap>();
        moveInfoTilemap.transform.SetParent(grid.transform);
        
        var attacktile = GameManager.Resource.Create<GameObject>(Path.Map + "AttackRangeTilemap");
        attackRangeTilemap = attacktile.GetComponent<Tilemap>();
        
        mapData = new int[mapWidth, mapHeight];
        mapCreator.GenerateMap(mapData, tilemap, groundTile, wallTile);
        
        attackRange.Initialize(attackRangeTilemap, redAttackTile, mainCamera, grid);

        pathfinding = new Pathfinding(tilemap);

        spawnController.SpawnAllObjects();
    }
    
    public void CreateMovePoint()
    {
        var temp = GameManager.Resource.Create<GameObject>(Path.Map + "Tilemap");
        var movePointTilemap = temp.GetComponent<Tilemap>();
        movePointTilemap.transform.SetParent(grid.transform);
    }
    
    public void SetObjectPosition(int x, int y, int objectID)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            mapData[x, y] = objectID;
        }
    }

    // 플레이어 이동 범위 업데이트
    public void PlayerUpdateRange(Vector3Int playerPos, int moveRange)
    {
        playerMoveInfo.ShowMoveInfoRange(playerPos, moveRange, moveInfoTile, moveInfoTilemap);

    }

    public void ClearPlayerRange()
    {
        //Debug.Log("Clear Player Range");
        playerMoveInfo.RemoveMoveInfoRange(moveInfoTilemap);
    }

    // 이동할 때
    public void UpdateObjectPosition(Vector3Int oldX, Vector3Int oldY, Vector3Int newX, Vector3Int newY, int objectID)
    {
        // 새로운 위치에 기록
        if (newX.x >= 0 && newY.y >= 0 && newX.x < mapWidth && newY.y < mapHeight)
        {
            mapData[newX.x, newY.y] = objectID;
        }
        // 이전 위치를 0으로
        if (oldX.x >= 0 && oldY.y >= 0 && oldX.x < mapWidth && oldY.y < mapHeight)
        {
            mapData[oldX.x, oldY.y] = TileID.Terrain;
        }
    }

    public void UpdateAttackTargets(List<Vector3Int> attackCells, List<BaseEnemy> enemies)
    {
        CurrentEnemyTargets.Clear();
        CurrentObstacleCoords.Clear();
        
        foreach (var cell in attackCells)
        {
            if (cell.x >= 0 && cell.x < mapData.GetLength(0) && cell.y >= 0 && cell.y < mapData.GetLength(1))
            {
                int tileID = mapData[cell.x, cell.y];

                if (tileID == TileID.Enemy)
                {

                    if (enemies == null)
                    {
                        Debug.LogError("MapManager의 enemies가 null");

                    }
                    else
                    {
                        Debug.Log($"enemies 리스트의 현재 몬스터 수: {enemies.Count}");

                        foreach (var enemy in enemies)
                        {
                            Vector3Int enemyCellPos = GetCellFromWorldPos(enemy.transform.position);

                            if (enemyCellPos == cell)
                            {
                                CurrentEnemyTargets.Add(enemy);

                                break;
                            }
                        }
                    }
                }
                else if (tileID == TileID.Obstacle)
                {
                    CurrentObstacleCoords.Add(cell);
                }
            }
        }

    }
    public Vector3Int GetCellFromWorldPos(Vector3 worldPosition)
    {
        if (grid == null)
        {
            Debug.LogError("Grid component is not assigned in MapManager!");
            return Vector3Int.zero;
        }
        
        return grid.WorldToCell(worldPosition);
    }
    
    public void ClearAttackTargets()
    {
        CurrentEnemyTargets.Clear();
        CurrentObstacleCoords.Clear();
    }
    // 턴끝 행동을 위한 기능
    public void RegisterTurnEndEffect(ITurnEndEffect effect)
    {
        if (!turnEndEffects.Contains(effect))
        {
            turnEndEffects.Add(effect);
        }
    }
    public void UnregisterTurnEndEffect(ITurnEndEffect effect)
    {
        turnEndEffects.Remove(effect);
    }
    //범위 판단
    public bool IsInside(Vector3Int c)
    {
        return c.x >= 0 && c.x < mapWidth && c.y >= 0 && c.y < mapHeight;
    }
    
    // 임시로 선언한 함수들

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int dest)
    {
        return pathfinding.FindPath(start, dest);
    }

    public Vector2Int GetPlayerPosition()
    {
        if (mapData == null)
        {
            Debug.Log("맵 데이터 없슴");
            
            return new Vector2Int(-1, -1);
        }

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
    public bool IsVehicle(Vector3Int cell)
    {
        return mapData[cell.x,cell.y] == TileID.Vehicle;
    }

    public bool IsPlayer(Vector3Int cell)
    {
        return mapData[cell.x, cell.y] == TileID.Player;
    }

    public bool IsObstacle_Breakable(Vector3Int cell)
    {
        return mapData[cell.x, cell.y] == TileID.Obstacle;
    }

    public bool IsEnemy(Vector3Int cell)
    {
        return mapData[cell.x, cell.y] == TileID.Enemy;
    }

}