using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPointObstacle : MonoBehaviour
{
    public GameObject obstaclePrefab; // 장애물 프리팹
    public int numberOfObstacles = 5; // 장애물 갯수

    // 장애물 ID
    const int OBSTACLE_ID = 4;

    public void SpawnObstacles(Tilemap tilemap)
    {
        int spawnedCount = 0;
        
        while (spawnedCount < numberOfObstacles)
        {
            // 랜덤
            int randX = Random.Range(0, GameManager.Map.mapWidth);
            int randY = Random.Range(0, GameManager.Map.mapHeight);

            // 해당 좌표가 빈 공간인지 확인 / 0,1은 빈칸(발판)
            if (GameManager.Map.mapData[randX, randY] == 0 || GameManager.Map.mapData[randX, randY] == 1)
            {
                // 장애물 프리팹 생성 /// 일단 애너미 프리펩 넣어둠 경로 다시 지정
                GameObject obstacleInstance = GameManager.Resource.Create<GameObject>(Path.Map + "Obstacle");

                // 장애물을 셀 중앙으로 이동
                GridSnapper.SnapToCellCenter(obstacleInstance.transform, tilemap, new Vector2Int(randX, randY));

                // 장애물 위치를 기록
                GameManager.Map.mapData[randX, randY] = OBSTACLE_ID;
                
                spawnedCount++;
            }
        }
    }
}
