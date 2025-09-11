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
            int randX = Random.Range(0, MapManager.instance.mapWidth);
            int randY = Random.Range(0, MapManager.instance.mapHeight);

            // 해당 좌표가 빈 공간인지 확인 / 0,1은 빈칸(발판)
            if (MapManager.mapData[randX, randY] == 0 || MapManager.mapData[randX, randY] == 1)
            {
                // 장애물 프리팹 생성
                GameObject obstacleInstance = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity);

                // 장애물을 셀 중앙으로 이동
                GridSnapper.SnapToCellCenter(obstacleInstance.transform, tilemap, new Vector2Int(randX, randY));

                // 장애물 위치를 기록
                MapManager.mapData[randX, randY] = OBSTACLE_ID;
                
                spawnedCount++;
            }
        }
    }
}
