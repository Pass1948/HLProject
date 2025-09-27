using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    private BasicObstaclePool obstaclePool;
    
    private GameObject enemyPrefab;
    private GameObject obstaclePrefab;
    
    // MapManager의 Start에서 호출
    public void InitializeSpawnersAndPools()
    {
        obstaclePool = gameObject.AddComponent<BasicObstaclePool>();
        

    }
    
    public void SpawnAllObjects(Stage stage)
    {
        enemyPrefab = GameManager.Resource.Load<GameObject>(Path.Enemy + "NormalEnemy");
        obstaclePrefab = GameManager.Resource.Load<GameObject>(Path.Map + "Obstacle");
        
        obstaclePool.prefab = obstaclePrefab;
        obstaclePool.InitializePool(20);
        
        SpawnPlayer();
        SpawnObstacles(5, stage.obstaclesDict);
        // SpawnEnemys(stage.enemiesDict); 
        SpawnEnemies(stage.enemiesDict, stage.eliteCnt, stage.id);
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
    
    // TODO: 장애물 구현하신거에 맞게 구현 해주세용
    // 장애물 스폰
    private void SpawnObstacles(int count, Dictionary<int, int> obstacles)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obstacle = obstaclePool.GetPooledObject();
            
            int maxAttempts = 100;
            for (int j = 0; j < maxAttempts; j++)
            {
                int randX = Random.Range(0, GameManager.Map.mapWidth);
                int randY = Random.Range(0, GameManager.Map.mapHeight);

                // 플레이어가 없는 공간
                if (GameManager.Map.mapData[randX, randY] == TileID.Terrain)
                {
                    //좌표 보정
                    GridSnapper.SnapToCellCenter(obstacle.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));
                    
                    GameManager.Map.SetObjectPosition(randX, randY, TileID.Obstacle);
                    break;
                }
            }
        }
    }
    
    // 적 스폰
    private void SpawnEnemies(Dictionary<int, int> enemies, int eliteCount, int stageId)
    {
        List<BaseEnemy> spawnedList = new List<BaseEnemy>();

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
                    spawnedList.Add(baseEnemy);

                    // break;
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