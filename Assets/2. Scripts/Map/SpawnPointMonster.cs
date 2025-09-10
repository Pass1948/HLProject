using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPointMonster : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public int numberOfMonsters = 3; // 몬스터 갯수

    // 몬스터 ID를 상수로 정의합니다.
    const int MONSTER_ID = 5;

    public void SpawnMonsters(Tilemap tilemap)
    {
        int spawnedCount = 0;
        
        while (spawnedCount < numberOfMonsters)
        {
            // 랜덤
            int randX = Random.Range(0, MapManager.instance.mapWidth);
            int randY = Random.Range(0, MapManager.instance.mapHeight);

            // 빈 공간인지 확인
            if (MapManager.mapData[randX, randY] == 0 || MapManager.mapData[randX, randY] == 1)
            {
                // 몬스터 프리팹 생성
                GameObject monsterInstance = Instantiate(monsterPrefab, Vector3.zero, Quaternion.identity);

                // 몬스터를 셀 중앙으로 이동
                GridSnapper.SnapToCellCenter(monsterInstance.transform, tilemap, new Vector2Int(randX, randY));

                // 몬스터 위치를 기록
                MapManager.mapData[randX, randY] = MONSTER_ID;
                
                spawnedCount++;
            }
        }
    }
}
