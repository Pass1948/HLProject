using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChecker : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera mainCamera; // 카메라 변수를 public으로 설정

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 화면의 마우스 위치에서 월드 공간으로 광선을 발사
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 2D 게임이라면 RaycastHit2D를 사용하는 것이 더 효율적입니다.
            // 3D 맵을 2.5D로 사용하는 경우에도 이 방법이 유효합니다.
            if (Physics.Raycast(ray, out hit))
            {
                // 광선이 부딪힌 지점의 월드 좌표를 가져옴
                Vector3 hitWorldPos = hit.point;

                // 월드 좌표를 그리드 좌표로 변환
                Vector3Int cellPos = tilemap.WorldToCell(hitWorldPos);
                
                Debug.Log($"클릭한 곳의 타일 좌표(Cell): ({cellPos.x}, {cellPos.y})");
            }
        }
    }
}
