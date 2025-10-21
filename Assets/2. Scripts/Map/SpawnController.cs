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
                GridSnapper.SnapToCellCenter(playerSpawn.transform, GameManager.Map.tilemap,
                    new Vector2Int(randX, randY));
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

    /// //////
    // 적 스폰
    // 
    private void SpawnEnemies(Dictionary<int, int> enemies, int eliteCount, int stageId)
    {
        List<BaseEnemy> spawnedList = new List<BaseEnemy>();
        
        foreach (var enemy in enemies)
        {
            int entityId = enemy.Key;
            int count = enemy.Value;

            // ------------------------------------
            // 보스 소환 로직 (2011 이상)
            // ------------------------------------
            if (entityId >= 2011)
            {
                // Entity ID에 따라 프리팹 이름 결정
                string bossPrefabName = "";
                switch (entityId)
                {
                    case 2011:
                        bossPrefabName = "Boss"; // 7008 Boss
                        break;
                    case 2012:
                        bossPrefabName = "Boss2"; // 7016 Boss
                        break;
                    case 2013:
                        bossPrefabName = "Boss3"; // 7024 Boss
                        break;
                    default:
                        Debug.LogError($"알 수 없는 보스 Entity ID: {entityId}입니다.");
                        continue;
                }

                // 보스 프리팹 로드
                bossPrefab = GameManager.Resource.Load<GameObject>(Path.Enemy + bossPrefabName);

                if (bossPrefab == null)
                {
                    Debug.LogError($"보스 프리팹 로드 실패: {Path.Enemy + bossPrefabName}. 경로와 파일명 확인 필요.");
                    continue;
                }

                Debug.Log($"보스 Entity ID {entityId} 확인. 프리팹 로드 성공.");

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
                Debug.Log(
                    $"보스 ID {entityId} ({bossPrefabName})가 ({BossX}, {BossY})에 성공적으로 소환되었습니다.");
                
                continue;
            }

            // ------------------------------------
            // 일반 적 소환 로직 (2001 ~ 2010)
            // ------------------------------------
            if (entityId >= 2001 && entityId <= 2010)
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

                            break;
                        }

                        tryCount++;
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