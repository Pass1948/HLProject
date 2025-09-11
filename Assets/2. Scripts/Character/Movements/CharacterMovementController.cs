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
    public  bool _isMoving = false;  // 이동 중 여부

    private Pathfinding _pathfinding;

    private void Awake()
    {
        // 시작 시 캐릭터를 타일 중심으로 스냅
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);

        // 경로 탐색기 초기화
        _pathfinding = new Pathfinding(tilemap);

    }
    private void Update()
    {
        
        if (TryGetMouseWorldOnGrid(out var mouseWorld))
        {
            var targetCell = tilemap.WorldToCell(mouseWorld);

            if (targetCell != _cellPosition)
            {
                var path = _pathfinding.FindPath(_cellPosition, targetCell);
            }
            else
            {
                
            }
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
        
        
    }

    /// <summary>
    /// 카메라 종류/그리드 평면에 맞춰 마우스가 찍은 '그리드 평면'상의 월드 좌표를 얻는다.
    /// </summary>
    private bool TryGetMouseWorldOnGrid(out Vector3 world)
    {
        var mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

        //XZ 평면
        Plane plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            world = ray.GetPoint(enter);
            return true;
        }
        world = default;
        return false;
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