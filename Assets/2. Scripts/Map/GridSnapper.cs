using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSnapper : MonoBehaviour
{
    public static void SnapToCellCenter(Transform targetTransform, Tilemap tilemap, Vector2Int newPos)
    {
        // 타일 좌표를 월드 좌표로 변환
        Vector3 newWorldPos = tilemap.CellToWorld(new Vector3Int(newPos.x, newPos.y, 0));

        // 타일 중앙으로 이동할 오프셋 계산 (X이 90 회전되어서 y는 높이 / 타일맵이 2D라서 z에 y값을 넣음)
        Vector3 cellSize = tilemap.cellSize;
        Vector3 centerOffset = new Vector3(cellSize.x / 2, 0, cellSize.y / 2);
        
        // 오브젝트의 실제 위치 보정
        Vector3 finalPos = new Vector3(newWorldPos.x + centerOffset.x, 0, newWorldPos.z + centerOffset.z);

        // 오브젝트의 Transform 위치를 업데이트
        targetTransform.position = finalPos;
    }
}
