using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;


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

    List<BaseBoss> bosses = new List<BaseBoss>();
    private GameObject bossPrefab;

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

        SpawnPlayer(stage);
        // SpawnEnemys(stage.enemiesDict); 

        SpawnEnemies(stage.enemiesDict, stage.eliteCnt, GameManager.Map.stageID);

        SpawnObstacles(stage.obstaclesDict, GameManager.Map.stageID);
    }

    private void SpawnPlayer(Stage stage)
    {
        // 튜토리얼 스테이지(7001)
        bool tutorialPlayerSpawn = (GameManager.Map.stageID == 7001);
        Vector2Int playerPos;
        Vector2Int vehiclePos;

        if (tutorialPlayerSpawn)
        {
            // 튜토리얼 고정 위치 플레이어 (2, 2), 오토바이 (1, 2)
            playerPos = new Vector2Int(2, 2);
            vehiclePos = new Vector2Int(1, 2);
        }
        else
        {
            // 기존 플레이어 위치 (1, 2)
            playerPos = new Vector2Int(1, 2);
            vehiclePos = new Vector2Int(2, 2);
        }
        
        if (GameManager.Map.mapData[playerPos.x, playerPos.y] != TileID.Terrain && !tutorialPlayerSpawn)
        {
            Debug.LogError("플레이어를 소환할 수 없습니다.");
            return;
        }
        
        // 플레이어 생성 
        GameObject playerSpawn = GameManager.Resource.Create<GameObject>(Path.Player + "Player");
        BasePlayer basePlayer = GameManager.Unit.Player.GetComponent<BasePlayer>(); 
        
        GameObject vehicleSpawn = GameManager.Resource.Create<GameObject>(Path.Player + "Vehicle");
        BaseVehicle baseVehicle = GameManager.Unit.Vehicle.GetComponent<BaseVehicle>(); 
        
        basePlayer.playerModel.InitData(GameManager.Data.entityDataGroup.GetEntityData(1001));
        basePlayer.controller.GetPosition(playerPos.x, playerPos.y); 
        
        baseVehicle.vehicleModel.InitData(GameManager.Data.entityDataGroup.GetEntityData(1501));
        //baseVehicle.controller.GetPosition(vehiclePos.x, vehiclePos.y); 
        
        // 좌표 보정 및 맵 데이터 업데이트
        GridSnapper.SnapToCellCenter(playerSpawn.transform, GameManager.Map.tilemap, playerPos); 
        GridSnapper.SnapToCellCenter(vehicleSpawn.transform, GameManager.Map.tilemap, vehiclePos);
        
        GameManager.Map.SetObjectPosition(playerPos.x, playerPos.y, TileID.Player);
        GameManager.Map.SetObjectPosition(vehiclePos.x, vehiclePos.y, TileID.Vehicle);

    }

    // 장애물 스폰
    public void SpawnObstacles(Dictionary<int, int> obstacles, int stageId)
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
            if (stageId == 7001)
            {
                const int FixedX = 5;
                const int MaxY = 9;
    
                // Y=0부터 Y=9까지 배치
                for (int y = 0; y <= MaxY; y++) 
                {
                    int x = FixedX;
                    Vector3Int spawnPos = new Vector3Int(x, y, 0);
        
                    // 맵 확인
                    if (x < GameManager.Map.mapWidth && y < GameManager.Map.mapHeight && 
                        GameManager.Map.mapData[x, y] == TileID.Terrain)
                    {
                        // 풀에서 오브젝트를 가져옴
                        GameObject obj = pool.GetPooledObject();

                        if (obj == null) continue;

                        obj.transform.SetParent(transform);
                        BaseObstacle baseObstacle = obj.GetComponent<BaseObstacle>();

                        if (baseObstacle != null)
                        {
                            baseObstacle.InitObstacle(spawnPos, obstacleData);
                            baseObstacle.SetPosition(spawnPos);

                            TurnEndDamageEffect damageEffect = obj.GetComponent<TurnEndDamageEffect>();
                            if (damageEffect != null)
                            {
                                damageEffect.SetupEffect();
                            }
                            
                            GameManager.Map.SetObjectPosition(x, y, TileID.Obstacle);
                
                            spawnedCount++;
                        }
                    }
                }
                spawnedCount = obstacleEntry.Value; 
            }
            else // 7001이 아닌 경우: 기존 랜덤 배치
            {

                for (int i = 0; i < maxAttempts && spawnedCount < obstacleEntry.Value; i++)
                {
                    int randX = Random.Range(0, GameManager.Map.mapWidth);
                    int randY = Random.Range(0, GameManager.Map.mapHeight);
                    Vector3Int spawnPos = new Vector3Int(randX, randY, 0);

                    if (GameManager.Map.mapData[randX, randY] == TileID.Terrain)
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
                        int tileIdToSet = TileID.Terrain;
                        if (obstacleData.canPlaceUnit == 0)
                        {
                            tileIdToSet = TileID.Obstacle;
                        }

                        GameManager.Map.SetObjectPosition(randX, randY, tileIdToSet);
                        spawnedCount++;

                    }
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

    /// //////
    // 적 스폰
    // 
    private void SpawnEnemies(Dictionary<int, int> enemies, int eliteCount, int stageId)
    {
        List<BaseEnemy> spawnedList = new List<BaseEnemy>();
        int enemySpawnedCount = 0;
        
        foreach (var enemy in enemies)
        {
            int entityId = enemy.Key;
            int count = enemy.Value;

            // ------------------------------------
            // 보스 소환 로직 (2014 이상)
            // ------------------------------------
            if (entityId == 2003 || entityId >= 2014)
            {
                // Entity ID에 따라 프리팹 이름 결정
                int requiredStageId = -1;
                string bossPrefabName = null;
                
                switch (entityId)
                {
                    case 2003:
                        requiredStageId = 7002;
                        bossPrefabName = "Boss"; // 7002 Boss (튜토리얼)
                        break;
                    case 2014:
                        requiredStageId = 7008;
                        bossPrefabName = "Boss2"; // 7008 Boss
                        break;
                    case 2015:
                        requiredStageId = 7016;
                        bossPrefabName = "Boss3"; // 7016 Boss
                        break;
                    default:
                        Debug.LogError($"알 수 없는 보스 Entity ID: {entityId}입니다.");
                        continue;
                }
                
                if (stageId != requiredStageId)
                {
                    continue; 
                }

                // 보스 프리팹 로드
                bossPrefab = GameManager.Resource.Load<GameObject>(Path.Enemy + bossPrefabName);

                if (bossPrefab == null)
                {
                    Debug.LogError($"보스 프리팹 로드 실패: {Path.Enemy + bossPrefabName}. 경로와 파일명 확인 필요.");
                    continue;
                }

                // 고정 좌표
                const int BossX = 7;
                const int BossY = 7;
                Vector2Int bossPos = new Vector2Int(BossX, BossY);

                GameObject obj = Instantiate(bossPrefab, transform);

                if (obj == null)
                {
                    Debug.LogError("GameObject 인스턴스화 실패.");
                    continue;
                }

                BaseBoss baseBoss = obj.GetComponent<BaseBoss>();

                if (baseBoss == null)
                {
                    Debug.LogError($"보스 프리팹 '{bossPrefabName}'에 BaseBoss 컴포넌트가 없습니다.");
                    Destroy(obj);
                    continue;
                }

                // 위치 설정 및 초기화
                GridSnapper.SnapToCellCenter(obj.transform, GameManager.Map.tilemap, bossPos);

                baseBoss.InitBoss(GameManager.Data.entityDataGroup.GetEntityData(entityId));
                baseBoss.controller.SetPosition(BossX, BossY);
                baseBoss.controller.UpdatePlayerPos();

                // 맵 데이터 업데이트 및 리스트 추가
                GameManager.Map.SetObjectPosition(BossX, BossY, TileID.Boss); 

                // bosses 리스트에 보스 추가
                bosses.Add(baseBoss);
                FindObjectOfType<AttackRangeDisplay>()?.bosses.Add(baseBoss);
                Debug.Log($"보스 성공 스폰: Entity ID = {entityId}");
                continue;
            }
            
            // ------------------------------------
            // 일반 적 소환 로직 (2004 ~ 2013)
            // ------------------------------------
            
            var tutorialEnemySpawn = new Vector2Int[]
            {
                new Vector2Int(3, 3),   // 첫 번째 적 (2001) 위치
                new Vector2Int(1, 9)  // 두 번째 적 (2002) 위치
            };
        
            
            
            // 튜토리얼 적 (2001,2002) / 일반 적 ID (2004-2013) 
            if ((entityId >= 2001 && entityId <= 2013) && entityId != 2003)
            { 
                // 스테이지 7001 고정 소환
                if (stageId == 7001)
                {
                    // count만큼 반복하며 고정된 위치에 소환
                    for (int i = 0; i < count; i++)
                    {
                        if (enemySpawnedCount < tutorialEnemySpawn.Length)
                        {
                            Vector2Int fixedPos = tutorialEnemySpawn[enemySpawnedCount];
                            
                            GameObject obj = Instantiate(enemyPrefab, transform);
                            BaseEnemy baseEnemy = obj.GetComponent<BaseEnemy>();

                            if (baseEnemy == null)
                            {
                                Debug.LogError($"에너미 프리팹에 BaseEnemy 컴포넌트가 없습니다. EntityID: {entityId}");
                                Destroy(obj);
                                enemySpawnedCount++;
                                continue;
                            }

                            GridSnapper.SnapToCellCenter(obj.transform, GameManager.Map.tilemap, fixedPos);

                            baseEnemy.InitEnemy(GameManager.Data.entityDataGroup.GetEntityData(entityId), EnemyType.Normal);
                            baseEnemy.controller.SetPosition(fixedPos.x, fixedPos.y); 
                            baseEnemy.controller.UpdatePlayerPos();
                            
                            GameManager.Map.SetObjectPosition(fixedPos.x, fixedPos.y, TileID.Enemy);
                            FindObjectOfType<AttackRangeDisplay>()?.enemies.Add(baseEnemy);
                            spawnedList.Add(baseEnemy);
                            // ----------------------------------------------------
                            Debug.Log($"튜토 적 성공 스폰: Entity ID = {entityId}");
                            enemySpawnedCount++; 
                        }
                        else
                        {
                            Debug.LogWarning($"튜토리얼 스테이지 {stageId}의 위치가 부족합니다");
                            break; 
                        }
                    }
                }
                ////
                // 7001이 아닌 일반 스테이지
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int tryCount = 0;
                        while (tryCount < 100)
                        {
                            int randX = Random.Range(0, GameManager.Map.mapWidth);
                            int randY = Random.Range(0, GameManager.Map.mapHeight);

                            if (GameManager.Map.mapData[randX, randY] == TileID.Terrain &&
                                !(randX >= 0 && randX <= 3 && randY >= 0 && randY <= 3))
                            {
                                GameObject obj = Instantiate(enemyPrefab, transform);
                                BaseEnemy baseEnemy = obj.GetComponent<BaseEnemy>();

                                GridSnapper.SnapToCellCenter(obj.transform, GameManager.Map.tilemap,
                                    new Vector2Int(randX, randY));

                                baseEnemy.InitEnemy(GameManager.Data.entityDataGroup.GetEntityData(entityId),
                                    EnemyType.Normal);
                                baseEnemy.controller.SetPosition(randX, randY);
                                baseEnemy.controller.UpdatePlayerPos();

                                GameManager.Map.SetObjectPosition(randX, randY, TileID.Enemy);

                                FindObjectOfType<AttackRangeDisplay>()?.enemies.Add(baseEnemy);
                                spawnedList.Add(baseEnemy);
                                Debug.Log($"일반 적 성공 스폰: Entity ID = {entityId}");
                                break;
                            }

                            tryCount++;
                        }
                    }
                }
            }
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
}