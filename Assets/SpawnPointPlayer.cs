using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPointPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject motorcyclePrefab;
    
    private Vector2Int playerSpawnPoint = new Vector2Int(3, 3);
    private Vector2Int motorcycleSpawnPoint  = new Vector2Int(3, 2);

    public void SpawnPlayer(Tilemap tilemap)
    {
        // 타일 좌표를 월드 좌표로
        Vector3 playerWorldPos = tilemap.CellToWorld(new Vector3Int(playerSpawnPoint.x, playerSpawnPoint.y, 0));
        Vector3 motorcycleWorldPos = tilemap.CellToWorld(new Vector3Int(motorcycleSpawnPoint.x, motorcycleSpawnPoint.y, 0));
        
        Vector3 cellSize = tilemap.cellSize;
        // x, z축으로 절반씩 이동
        Vector3 centerOffset = new Vector3(cellSize.x / 2, 0, cellSize.y / 2);
        
        // 월드 좌표에 적용
        playerWorldPos += centerOffset;
        motorcycleWorldPos += centerOffset;

        // 오브젝트 생성 // y는 높이
        Instantiate(playerPrefab, new Vector3(playerWorldPos.x, 0, playerWorldPos.z), Quaternion.identity);
        Instantiate(motorcyclePrefab, new Vector3(motorcycleWorldPos.x, 0, motorcycleWorldPos.z), Quaternion.identity);
    }
}
