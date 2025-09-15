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
        GameManager.Map.mapData[playerSpawnPoint.x, playerSpawnPoint.y] = TileID.Player;
        GameManager.Map.mapData[motorcycleSpawnPoint.x, motorcycleSpawnPoint.y] = TileID.Motor;
        
        // 플레이어 오브젝트 생성
        GameObject playerInstance = GameManager.Resource.Create<GameObject>(Path.Player + "Player");
        MovementController player = playerInstance.GetComponent<MovementController>();
        Debug.Log(player);
        player.Init(tilemap);
        GridSnapper.SnapToCellCenter(playerInstance.transform, tilemap, playerSpawnPoint);

        // 오토바이 오브젝트 생성 // 일단 플레이어 프리펩으로 넣어둠 경로 다시 설정
        //GameObject motorcycleInstance = GameManager.Resource.Create<GameObject>(Path.Player + "Player");
        //MovementController motor= playerInstance.GetComponent<MovementController>();
        //Debug.Log(motor);
        //motor.Init(tilemap);
        //GridSnapper.SnapToCellCenter(motorcycleInstance.transform, tilemap, motorcycleSpawnPoint);
    }
}
