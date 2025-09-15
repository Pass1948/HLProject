using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    private MonsterPool monsterPool;
    private BasicObstaclePool obstaclePool;
    
    private GameObject monsterPrefab;
    private GameObject obstaclePrefab;
    
    // MapManager의 Awake에서 호출
    public void InitializeSpawnersAndPools()
    {
        monsterPool = gameObject.AddComponent<MonsterPool>();
        obstaclePool = gameObject.AddComponent<BasicObstaclePool>();
        
        monsterPrefab = GameManager.Resource.Load<GameObject>(Path.Enemy + "Enemy");
        obstaclePrefab = GameManager.Resource.Load<GameObject>(Path.Map + "Obstacle");
        
        monsterPool.prefab = monsterPrefab;
        obstaclePool.prefab = obstaclePrefab;
        
        monsterPool.InitializePool(10);
        obstaclePool.InitializePool(20);
    }

    // 모든 오브젝트 스폰을 총괄하는 함수
    public void SpawnAllObjects()
    {
        SpawnPlayer();
        SpawnObstacles(10);
        SpawnMonsters(5);
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
                GameObject player = GameManager.Resource.Create<GameObject>(Path.Player + "Player");
                //좌표 보정
                GridSnapper.SnapToCellCenter(player.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));
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

    // 몬스터 스폰
    private void SpawnMonsters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject monster = monsterPool.GetPooledObject();

            int maxAttempts = 100;
            for (int j = 0; j < maxAttempts; j++)
            {
                int randX = Random.Range(0, GameManager.Map.mapWidth);
                int randY = Random.Range(0, GameManager.Map.mapHeight);

                // 플레이어, 장애물이 없는 곳 / (0,0) - (3,3) 아닌 곳
                if (GameManager.Map.mapData[randX, randY] == TileID.Terrain &&
                    !(randX >= 0 && randX <= 3 && randY >= 0 && randY <= 3))
                {
                    //좌표 보정
                    GridSnapper.SnapToCellCenter(monster.transform, GameManager.Map.tilemap, new Vector2Int(randX, randY));
                    
                    GameManager.Map.SetObjectPosition(randX, randY, TileID.Enemy);
                    break;
                }
            }
        }
    }
}