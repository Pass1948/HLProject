using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPointMonster : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public int numberOfMonsters = 2; // 몬스터 갯수

    // 몬스터 ID
    const int MONSTER_ID = 5;

    public void SpawnMonsters(Tilemap tilemap)
    {
        int spawnedCount = 0;
        
        while (spawnedCount < numberOfMonsters)
        {
            // 랜덤
            int randX = Random.Range(0, GameManager.Map.mapWidth);
            int randY = Random.Range(0, GameManager.Map.mapHeight);
            Debug.Log($"randomPosition {randX}, {randY}");
            // 해당 좌표가 빈 공간인지 확인 / 0,1은 빈칸(발판)
            if (GameManager.Map.mapData[randX, randY] == 0 || GameManager.Map.mapData[randX, randY] == 1)
            {
                // 몬스터 프리팹 생성
                GameObject monsterInstance = GameManager.Resource.Create<GameObject>(Path.Enemy + "Enemy");
                var controller = monsterInstance.GetComponent<EnemyController>();
                // 몬스터를 셀 중앙으로 이동
                GridSnapper.SnapToCellCenter(monsterInstance.transform, tilemap, new Vector2Int(randX, randY));

                // 몬스터 위치 기록
                GameManager.Map.SetObjectPosition(randX, randY, TileID.Enemy);
                controller.SetPosition(randX, randY);
                controller.InitTarget();
                spawnedCount++;
            }
        }
    }
}
