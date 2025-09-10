using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class CharacterMovementController : MonoBehaviour 
{
    public enum GridPlane {XY,XZ }
    [Header("Grid Settings")]
    [SerializeField] private Tilemap tilemap; // 이동 기준 타일맵
    [SerializeField] private GridPlane gridPlane = GridPlane.XZ; // 그리드 평면 설정
    [SerializeField] private float groundY = 0f; // 그리드 셀 크기

    [Header("Movement Settings")]
    [SerializeField] private float moveTime = 0.2f;

    private Vector3Int _cellPosition; // 현재 캐릭터가 있는 타일 좌표
    private bool _isMoving = false;  // 이동 중 여부

    private Pathfinding _pathfinding;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        // 시작 시 캐릭터를 타일 중심으로 스냅
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);

        // 경로 탐색기 초기화
        _pathfinding = new Pathfinding(tilemap);

        // 라인 렌더러 초기화...
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.widthMultiplier = 0.1f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.green;

    }
    private void Update()
    {
        if(TryGetMouseWorldOnGrid(out var mouseWorld))
        {
            var targetCell = tilemap.WorldToCell(mouseWorld);

            if(targetCell != _cellPosition)
            {
                var path = _pathfinding.FindPath(_cellPosition, targetCell);
                DrowPath(path);
            }
            else
            {
                _lineRenderer.positionCount = 0;
            }
        }
    }

    private void DrowPath(List<Vector3Int> path)
    {
        if (path.Count < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }
        _lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            _lineRenderer.SetPosition(i, tilemap.GetCellCenterWorld(path[i]) + Vector3.up * 0.1f);
        }
    }




    private void OnMovementClick(InputValue value)
    {
        // UI 위 클릭은 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 버튼 액션이므로 pressed가 아니면 무시
        if (!value.isPressed) return;

        // 마우스가 가리키는 '그리드 평면' 위의 월드 좌표를 구한다.
        if (!TryGetMouseWorldOnGrid(out var mouseWorld)) return;

        // 그 월드 좌표를 셀 좌표로 변환
        var targetCell = tilemap.WorldToCell(mouseWorld);

        // 같은 칸이면 할 일 없음
        if (targetCell == _cellPosition) return;

        // A* 경로 찾기
        List<Vector3Int> path = _pathfinding.FindPath(_cellPosition, targetCell);

        if (path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
        }
        _lineRenderer.positionCount = 0;
        
    }

    /// <summary>
    /// 카메라 종류/그리드 평면에 맞춰 마우스가 찍은 '그리드 평면'상의 월드 좌표를 얻는다.
    /// </summary>
    private bool TryGetMouseWorldOnGrid(out Vector3 world)
    {
        var mousePos = Mouse.current.position.ReadValue();

        // Orthographic(2D Top-Down) 카메라일 경우: 깊이 개념이 없어서 z(또는 y)를 평면에 딱 맞춰준다.
        if (Camera.main.orthographic)
        {
            // nearClipPlane을 넣어도 orthographic에서는 상관없음
            world = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

            if (gridPlane == GridPlane.XY)
            {
                // 2D 타일맵: XY 평면 → z를 타일맵(혹은 캐릭터)의 z로 고정
                world.z = tilemap.transform.position.z;
            }
            else
            {
                // 3D 바닥 그리드: XZ 평면 → y를 바닥 높이로 고정
                world.y = groundY;
            }
            return true;
        }
        else
        {
            // Perspective(원근) 카메라: 반드시 레이로 '그리드 평면'에 교차시켜야 정확함
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (gridPlane == GridPlane.XY)
            {
                // XY 평면(2D 타일맵)을 월드에서 z=const 로 사용 중이라 가정
                // → z가 특정 값인 평면과의 교차점을 구한다.
                // 하지만 Unity의 Plane은 법선+점으로 정의 → z 고정 평면은 (0,0,1) 법선을 이용
                Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, tilemap.transform.position.z));
                if (plane.Raycast(ray, out float enter))
                {
                    world = ray.GetPoint(enter);
                    return true;
                }
            }
            else
            {
                // XZ 평면(3D): y=groundY인 평면과의 교차
                Plane plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
                if (plane.Raycast(ray, out float enter))
                {
                    world = ray.GetPoint(enter);
                    return true;
                }
            }

            world = default;
            return false;
        }
    }

    private IEnumerator FollowPath(List<Vector3Int> path)
    {
        foreach (var cell in path)
            yield return MoveRoutine(cell);
    }

    private IEnumerator MoveRoutine(Vector3Int targetCell)
    {
        _isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = tilemap.GetCellCenterWorld(targetCell);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        _cellPosition = targetCell;
        _isMoving = false;
    }
}