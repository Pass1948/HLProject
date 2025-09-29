using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;


public class SpawnController : MonoBehaviour
{
    //private BasicObstaclePool obstaclePool;
    private Dictionary<string, BasicObstaclePool> obstaclePools = new Dictionary<string, BasicObstaclePool>();
    private readonly string[] allPrefabNames = { 
        "Obstacle_Pillar", 
        "Obstacle_Blizzard" 
        //
    };
    
    private GameObject enemyPrefab;
    
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
        SpawnEnemies(stage.enemiesDict);
        SpawnObstacles(stage.obstaclesDict);
    }
    
    private void SpawnPlayer()
    {
        int count = 100;
        for (int i = 0; i < count; i++)
        {
            // (0,0) - (3,3) 플레이어 생성 범위
            int randX = Random.Range(0, 4);
            int randY = Random.Range(0, 4);

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
            Debug.LogWarning($"{spawnedCount}개만 스폰했습니다");
        }
    }
}
    
    // 적 스폰
    private void SpawnEnemies(Dictionary<int, int> enemies)
    {
        foreach (var enemy in enemies)
        {
            for (int i = 0; i < enemy.Value; i++)
            {
                int randX = Random.Range(0, GameManager.Map.mapWidth);
                int randY = Random.Range(0, GameManager.Map.mapHeight);

                if (GameManager.Map.mapData[randX, randY] == TileID.Terrain &&
                    !(randX >= 0 && randX <= 3 && randY >= 0 && randY <= 3))
                {
                    GameObject obj =  Instantiate(enemyPrefab, transform);
                    BaseEnemy baseEnemy = obj.GetComponent<BaseEnemy>();

                    GridSnapper.SnapToCellCenter(obj.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));
                    
                    baseEnemy.InitEnemy(GameManager.Data.entityDataGroup.GetEntityData(enemy.Key), EnemyType.Normal);
                    baseEnemy.controller.SetPosition(randX, randY);
                    baseEnemy.controller.UpdatePlayerPos();
                    
                    GameManager.Map.SetObjectPosition(randX, randY, TileID.Enemy);
                    
                    FindObjectOfType<AttackRangeDisplay>().enemies.Add(baseEnemy);

                    // break;
                }
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
    
}