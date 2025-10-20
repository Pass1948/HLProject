using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;
using System.Linq;


public class SpawnController : MonoBehaviour
{
    public Dictionary<string, BasicObstaclePool> obstaclePools = new Dictionary<string, BasicObstaclePool>();

    private readonly string[] allPrefabNames =
    {
        "Obstacle_Pillar",
        //"Obstacle_Blizzard" 
        //
    };
    
    private GameObject enemyPrefab;

    private GameObject golemPrefab;
    // MapManager의 Start에서 호출
    public void InitializeSpawnersAndPools()
    {
        foreach (var name in allPrefabNames)
        {
            GameObject prefab = GameManager.Resource.Load<GameObject>(Path.Obstacle + name);
            if (prefab != null)
            {
                BasicObstaclePool pool = gameObject.AddComponent<BasicObstaclePool>();
                pool.prefab = prefab;
                pool.InitializePool(10); // 풀마다 5개씩
                obstaclePools.Add(name, pool);
            }
        }
        
    }
    
    public void SpawnAllObjects(Stage stage)
    {
        enemyPrefab = GameManager.Resource.Load<GameObject>(Path.Enemy + "NormalEnemy");
        //obstaclePool.InitializePool(20);
        
        SpawnPlayer();
        // SpawnEnemys(stage.enemiesDict); 

        SpawnEnemies(stage.enemiesDict, stage.eliteCnt, stage.id);

        SpawnObstacles(stage.obstaclesDict);
    }
    
    private void SpawnPlayer()
    {
        int count = 100;
        for (int i = 0; i < count; i++)
        {
            // (1,1) - (2,2) 플레이어 생성 범위
            int randX = Random.Range(1, 2);
            int randY = Random.Range(1, 2);

            if (GameManager.Map.mapData[randX, randY] == TileID.Terrain)
            {
                // 플레이어 생성
                GameObject playerSpawn = GameManager.Resource.Create<GameObject>(Path.Player + "Player");
                BasePlayer basePlayer = GameManager.Unit.Player.GetComponent<BasePlayer>();

                GameObject vehicleSpawn = GameManager.Resource.Create<GameObject>(Path.Player + "Vehicle");

                BaseVehicle baseVehicle = GameManager.Unit.Vehicle.GetComponent<BaseVehicle>();

                basePlayer.playerModel.InitData(GameManager.Data.entityDataGroup.GetEntityData(1001));
                basePlayer.controller.GetPosition(randX, randY);

                baseVehicle.vehicleModel.InitData(GameManager.Data.entityDataGroup.GetEntityData(1501));

                //좌표 보정
                GridSnapper.SnapToCellCenter(playerSpawn.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));
                // 맵 데이터에 기록
                GameManager.Map.SetObjectPosition(randX, randY, TileID.Player);
                return;
            }
        }

    }
    
    /// ////////////////////
    // 장애물 스폰
    public void SpawnObstacles(Dictionary<int, int> obstacles)
    {
        int maxAttempts = GameManager.Map.mapWidth * GameManager.Map.mapHeight * 2; 
        foreach (var obstacleEntry in obstacles)
        {
            int spawnedCount = 0;
            int obstacleId = obstacleEntry.Key;
      
            ObstacleData obstacleData = GameManager.Data.obstacleDataGroup.GetObstacleData(obstacleId);
            if (obstacleData == null)
            {
                continue;
            }
            
            string prefabName = GetObstaclePrefabName(obstacleData.type);
            
            if (string.IsNullOrEmpty(prefabName))
            {
                continue;
            }
            
            // 현재 장애물 타입에 맞는 풀을 찾음
            if (!obstaclePools.TryGetValue(prefabName, out BasicObstaclePool pool))
            {
                continue;
            }
            
            for (int i = 0; i < maxAttempts && spawnedCount < obstacleEntry.Value; i++) 
            {
                int randX = Random.Range(0, GameManager.Map.mapWidth);
                int randY = Random.Range(0, GameManager.Map.mapHeight);
                Vector3Int spawnPos = new Vector3Int(randX, randY, 0);
      
                if (GameManager.Map.mapData[randX, randY] == (int)TileID.Terrain)
                {
                    if (CheckAdjacentObstacle(randX, randY))
                    {
                        // 주변에 장애물이 이미 있으면 다시 찾기
                        continue; 
                    }
                    
                    // 풀에서 오브젝트를 가져옴
                    GameObject obj = pool.GetPooledObject();
               
                    // 정상적으로 리턴되었는지 확인
                    if (obj == null)
                    {
                        continue;
                    } 
              
                    // 풀에서 가져온 오브젝트의 위치와 활성 상태를 초기화
                    obj.transform.SetParent(transform);
                    obj.SetActive(true);
              
                    BaseObstacle baseObstacle = obj.GetComponent<BaseObstacle>();
             
                    baseObstacle.InitObstacle(spawnPos, obstacleData); 
                    baseObstacle.SetPosition(spawnPos);
                    
                    TurnEndDamageEffect damageEffect = obj.GetComponent<TurnEndDamageEffect>();
                    
                    if (damageEffect != null)
                    {
                        damageEffect.SetupEffect(); 
                    }
                
               
                    // TileID 설정
                    int tileIdToSet = (int)TileID.Terrain; 
                    if (obstacleData.canPlaceUnit == 0)
                    {
                        tileIdToSet = (int)TileID.Obstacle; 
                    }
                    
                    GameManager.Map.SetObjectPosition(randX, randY, tileIdToSet);
                    spawnedCount++;

                }
            }
     
            if (spawnedCount < obstacleEntry.Value)
            {
                Debug.LogWarning($"{spawnedCount}개 스폰");
            }
        }
        
    }
    
    private string GetObstaclePrefabName(ObstacleType type)
    {
        switch (type)
        {
            // 사용할 타입만
            case ObstacleType.StonePillar:
                return "Obstacle_Pillar"; // 기둥류

            case ObstacleType.BlizzardZone:
                return "Obstacle_Blizzard"; // 눈보라 구역
        
            // 나머지 모든 타입은 default
            default:
                
                return string.Empty; 
        }
    }
    
    private bool CheckAdjacentObstacle(int x, int y)
    {
        // 주변 8방향
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int checkX = x + dx[i];
            int checkY = y + dy[i];

            // 맵 경계 체크
            if (checkX < 0 || checkX >= GameManager.Map.mapWidth || 
                checkY < 0 || checkY >= GameManager.Map.mapHeight)
            {
                continue;
            }

            // 주변 타일 장애물 인지
            if (GameManager.Map.mapData[checkX, checkY] == TileID.Obstacle)
            {
                return true; //
            }
        }
        // 배치 가능
        return false;
    }
    
    public void ReturnAllObstaclesToPool()
    {
        int returnedCount = 0;
        
        foreach (var poolEntry in obstaclePools)
        {
            BasicObstaclePool pool = poolEntry.Value;
            
            List<GameObject> objectsToReturn = new List<GameObject>();
            
            for (int i = 0; i < pool.transform.childCount; i++)
            {
                GameObject child = pool.transform.GetChild(i).gameObject;
                
                if (child.activeSelf)
                {
                    objectsToReturn.Add(child);
                }
            }
            
            foreach (GameObject obj in objectsToReturn)
            {
                pool.ReturnObjectToPool(obj);
                returnedCount++;
            }
        }
    }

    /////////////////
    // 적 스폰
    private void SpawnEnemies(Dictionary<int, int> enemies, int eliteCount, int stageId)
    {
        List<BaseEnemy> spawnedList = new List<BaseEnemy>();
        List<int> finalEnemyList = new List<int>();

        foreach (var enemyEntry in enemies)
        {
            for (int i = 0; i < enemyEntry.Value; i++)
            {
                finalEnemyList.Add(enemyEntry.Key);
            }
        }

        // 사이즈가 큰 몬스터부터 배치
        finalEnemyList = finalEnemyList.OrderByDescending(
            id => GameManager.Data.entityDataGroup.GetEntityData(id).size
        ).ToList();
        
        foreach (int enemyId in finalEnemyList)
        {
            // 사이즈를 고려한 스폰을 시도, 성공 시 spawnedList에 추가
            TrySpawnOneEnemyWithSize(enemyId, spawnedList);
        }

        if (eliteCount > 0 && spawnedList.Count > 0)
        {
            for (int i = 0; i < eliteCount; i++)
            {
                int index = Random.Range(0, spawnedList.Count);
                BaseEnemy elite = spawnedList[index];
                spawnedList.RemoveAt(index);

                elite.SetElite(stageId);
            }
        }
    }
    
    // 단일 몬스터의 스폰 시도
    private void TrySpawnOneEnemyWithSize(int entityId, List<BaseEnemy> spawnedList)
    {
        // 사이즈 
        int size = GameManager.Data.entityDataGroup.GetEntityData(entityId).size;
        if (size <= 0) size = 1; // 최소 1x1

        // 다중 타일 공간 탐색
        Vector3Int spawnPos = FindMultiTileSpawnPosition(size);
        
        if (spawnPos != Vector3Int.zero)
        {
            // 몬스터 생성 및 맵 데이터 업데이트
            SpawnOneEnemyWithSize(entityId, spawnPos, size, spawnedList);
        }
        else
        {
            Debug.LogWarning($"몬스터ID: {entityId}) 스폰 실패");
        }
    }
    
    // 실제 몬스터 생성, 위치/크기 , 맵 데이터 업데이트
    private void SpawnOneEnemyWithSize(int entityId, Vector3Int pos, int size, List<BaseEnemy> spawnedList)
    {
        // 몬스터 생성
        GameObject obj =  Instantiate(enemyPrefab, transform);
        BaseEnemy baseEnemy = obj.GetComponent<BaseEnemy>();

        // 몬스터의 위치 계산
        Vector2Int gridPos = new Vector2Int(pos.x, pos.y);
        
        // 사이즈가 1보다 클 경우 중앙 보정 (2x2는 1칸, 3x3은 2칸 중앙)
        if (size > 1)
        {
            // 2x2 유닛은 좌측 하단 타일 중앙이 아닌, 2x2 영역의 중앙으로 위치 보정
            // GridSnapper는 1x1 타일 기준
            
            // 2x2의 중심은 (1,1) 타일 중앙
            // 1x1 타일 중앙을 기준으로 (size - 1) / 2 만큼 이동
            float offset = (size - 1) / 2.0f; 
            
            Vector3 centerWorldPos = GameManager.Map.tilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
            
            centerWorldPos.x += offset * GameManager.Map.tilemap.cellSize.x;
            centerWorldPos.y += offset * GameManager.Map.tilemap.cellSize.y;
            
            obj.transform.position = centerWorldPos;
            
        }
        else
        {
            GridSnapper.SnapToCellCenter(obj.transform, GameManager.Map.tilemap, gridPos);
        }
        
        baseEnemy.InitEnemy(GameManager.Data.entityDataGroup.GetEntityData(entityId), EnemyType.Normal);
        baseEnemy.controller.SetPosition(pos.x, pos.y);
        baseEnemy.controller.UpdatePlayerPos();
        
        FindObjectOfType<AttackRangeDisplay>().enemies.Add(baseEnemy);
        spawnedList.Add(baseEnemy);
        
        // 맵 데이터 업데이트 TileID.Enemy
        UpdateMapDataForEnemy(pos, size, TileID.Enemy);
    }
    
    // 몬스터의 크기만큼 맵 데이터를 업데이트
    private void UpdateMapDataForEnemy(Vector3Int pos, int size, int tileId)
    {
        // size 영역을 tileId로 설정
        for (int x = pos.x; x < pos.x + size; x++)
        {
            for (int y = pos.y; y < pos.y + size; y++)
            {
                GameManager.Map.SetObjectPosition(x, y, tileId);
            }
        }
    }
    
    // size에 맞게 배치 가능한지 확인
    private bool IsSpawnAreaAvailable(int startX, int startY, int size)
    {
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;

        for (int x = startX; x < startX + size; x++)
        {
            for (int y = startY; y < startY + size; y++)
            {
                // 맵 경계 체크
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                {
                    return false;
                }
                
                // 타일이 이동 가능한 Terrain인지 확인
                if (GameManager.Map.mapData[x, y] != TileID.Terrain)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 다중 타일 몬스터 배치 가능한 좌표 탐색
    private Vector3Int FindMultiTileSpawnPosition(int size)
    {
        List<Vector3Int> potentialSpots = new List<Vector3Int>();
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;

        // size 영역을 확인
        for (int x = 0; x <= mapWidth - size; x++)
        {
            for (int y = 0; y <= mapHeight - size; y++)
            {
                // 플레이어 스폰 금지 영역 체크
                bool isNearPlayerStart = false;
                for (int sx = x; sx < x + size; sx++)
                {
                    for (int sy = y; sy < y + size; sy++)
                    {
                        if (sx >= 0 && sx <= 3 && sy >= 0 && sy <= 3) 
                        {
                            isNearPlayerStart = true;
                            break;
                        }
                    }
                    if (isNearPlayerStart) break;
                }
                
                if (isNearPlayerStart)
                {
                    continue;
                }

                if (IsSpawnAreaAvailable(x, y, size))
                {
                    potentialSpots.Add(new Vector3Int(x, y, 0)); 
                }
            }
        }

        if (potentialSpots.Count > 0)
        {
            // 무작위
            int randomIndex = Random.Range(0, potentialSpots.Count);
            return potentialSpots[randomIndex];
        }

        return Vector3Int.zero; // 배치할 수 있는 공간 없음
    }
    
    
}