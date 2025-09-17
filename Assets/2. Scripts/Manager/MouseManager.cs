using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    //[HideInInspector] 테스터 이후 가림
    public GameObject mouse;
    // 타일맵 위에서 타일과 오브젝트 탐색하는 탐색기 역할 매니저
    // 오브젝트가 마우스에 따라다니도록 하는 기능 구현 필요


    bool isActive = false;
    // 레이 맞출 레이어(예: Ground, TileMap 등만 체크)
    [SerializeField] private LayerMask groundMask = ~0;

    // 타일맵에 셀 스냅할지 여부(선택)
    [SerializeField] private bool snapToTilemap = true;
    [SerializeField] private Tilemap tilemap;      // Cell Swizzle = XZY 유지
    [SerializeField] private float groundY = 0f;   // XZ 평면 높이 고정

    // 부드럽게 이동할지 여부
    [SerializeField] private bool smooth = true;
    [SerializeField] private float smoothTime = 0.06f;

    // 내부용 스무딩 속도
    private Vector3 _vel;



    private readonly List<GameObject> activeTiles = new List<GameObject>();

    private void OnEnable()
    {
        mouse = GameManager.Resource.Create<GameObject>(Path.Mouse + "Pointer");
        mouse.transform.SetParent(this.transform);
    }


    private void LateUpdate()
    {
        FollowMouse();
    }


    private void FollowMouse()
    {
        var mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0, QueryTriggerInteraction.Ignore))// RaycastHit의 최대거리 설정으로 무한정 오류를 방지 
        {

        }
    }

    

    public void ShowPath(List<Vector3Int> path, Tilemap tilemap, int moveRange)
    {
        ClearPath();

        // 이동 범위 제한
        int maxRange = Mathf.Min(path.Count, moveRange);
        for (int i = 0; i < path.Count; i++)
        {
            if (tilemap == null) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(path[i]);
            if (i < moveRange)
            {
                var tileObj = GameManager.Resource.Create<GameObject>(Path.Map + "WhiteTile");
                tileObj.transform.position = worldPos;
                activeTiles.Add(tileObj);
            }
            else
            {
                var tileObj = GameManager.Resource.Create<GameObject>(Path.Map + "RedTile");
                tileObj.transform.position = worldPos;
                activeTiles.Add(tileObj);
            }
        }

    }

    public void ClearPath()
    {
        foreach (var tile in activeTiles)
        {
            Destroy(tile);
        }
        activeTiles.Clear();
    }
}
