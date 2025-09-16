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
        
        enemyPrefab = GameManager.Resource.Load<GameObject>(Path.Enemy + "Enemy");
        obstaclePrefab = GameManager.Resource.Load<GameObject>(Path.Map + "Obstacle");
        
        obstaclePool.prefab = obstaclePrefab;
        obstaclePool.InitializePool(20);
    }
    
    public void SpawnAllObjects()
    {
        SpawnPlayer();
        SpawnObstacles(10);
        SpawnEnemys(5);
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

                basePlayer.playerModel.InitData(GameManager.Data.GetUnit(UnitType.Player, 1001));
                basePlayer.controller.GetPosition(randX, randY);
                
                baseVehicle.vehicleModel.InitData(GameManager.Data.GetUnit(UnitType.Vehicle, 1501));
                baseVehicle.transform.parent = basePlayer.transform;

                //좌표 보정
                GridSnapper.SnapToCellCenter(playerSpawn.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));
                // 맵 데이터에 기록
                GameManager.Map.SetObjectPosition(randX, randY, TileID.Player);
                return;
            }
        }
    }
    
    // 장애물 스폰
    private void SpawnObstacles(int count)
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
    private void SpawnEnemys(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemy =  Instantiate(enemyPrefab, transform);
            BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
            
            int maxAttempts = 100;
            for (int j = 0; j < maxAttempts; j++)
            {
                int randX = Random.Range(0, GameManager.Map.mapWidth);
                int randY = Random.Range(0, GameManager.Map.mapHeight);

                // 플레이어, 장애물이 없는 곳 / (0,0) - (3,3) 아닌 곳
                if (GameManager.Map.mapData[randX, randY] == TileID.Terrain &&
                    !(randX >= 0 && randX <= 3 && randY >= 0 && randY <= 3))
                {
                    //좌표 
                    GridSnapper.SnapToCellCenter(enemy.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));

                    baseEnemy.InitEnemy(GameManager.Data.GetUnit(UnitType.Enemy, Random.Range(2001, 2010)));
                    baseEnemy.controller.SetPosition(randX, randY);
                    baseEnemy.controller.UpdatePlayerPos();

                    GameManager.Map.SetObjectPosition(randX, randY, TileID.Enemy);
                    break;
                }
            }
        }
    }
}