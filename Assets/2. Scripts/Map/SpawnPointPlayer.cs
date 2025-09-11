using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPointPlayer : MonoBehaviour
{
    public GameObject playerPrefab; // 플레이어 프리팹
    public GameObject motorcyclePrefab; // 오토바이 프리팹
    
    // 플레이어 ID, 오토바이 ID
    const int PLAYER_ID = 2;
    const int MOTORCYCLE_ID = 3;
    
    private Vector2Int playerSpawnPoint = new Vector2Int(3, 3);
    private Vector2Int motorcycleSpawnPoint  = new Vector2Int(3, 2);

    public void SpawnPlayer(Tilemap tilemap)
    {
        // 맵 데이터에 플레이어와 오토바이 위치 넣기
        MapManager.mapData[playerSpawnPoint.x, playerSpawnPoint.y] = PLAYER_ID;
        MapManager.mapData[motorcycleSpawnPoint.x, motorcycleSpawnPoint.y] = MOTORCYCLE_ID;

        // 플레이어 오브젝트 생성
        GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        GridSnapper.SnapToCellCenter(playerInstance.transform, tilemap, playerSpawnPoint);

        // 오토바이 오브젝트 생성
        GameObject motorcycleInstance = Instantiate(motorcyclePrefab, Vector3.zero, Quaternion.identity);
        GridSnapper.SnapToCellCenter(motorcycleInstance.transform, tilemap, motorcycleSpawnPoint);
    }
}
