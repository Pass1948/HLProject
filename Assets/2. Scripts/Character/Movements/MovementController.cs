using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class MovementController : MonoBehaviour 
{
    public enum GridPlane {XY,XZ }

    [Header("Grid Settings")]
    [SerializeField] private Tilemap tilemap; // 이동 기준 타일맵
    [SerializeField] private GridPlane gridPlane = GridPlane.XZ; // 그리드 평면 설정
    [SerializeField] private float groundY = 0f; // 그리드 셀 크기

    [Header("Movement Settings")]
    [SerializeField] private float moveTime = 0.2f;

    private Vector3Int _cellPosition; // 플레이어 현재 위치
    public  bool _isMoving = false;  // 움직임 감지

    private Pathfinding _pathfinding;

    private void Awake()
    {
        // 플레이어 시작 위치를 타일의 중앙으로 설정
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);

        // A* 알고리즘 초기화
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
                GameManager.PathPreview.ShowPath(path, tilemap);
            }
            else
            {
                
            }
        }
    }


    private void OnMovementClick(InputValue value)
    {
        // UI 가 있어도 클릭 안되게
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        GameManager.PathPreview.ClearPath();

        // 마우스 눌렀다가 땠을 때에는 처리 하지 않음
        if (!value.isPressed) return;

        // 
        if (!TryGetMouseWorldOnGrid(out var mouseWorld)) return;

        // 마우스 위치를 셀 위치로 변환
        var targetCell = tilemap.WorldToCell(mouseWorld);

        // 위치의 변화가 없거나 같으면 무시
        if (targetCell == _cellPosition) return;

        // A* 알고리즘 경로 설정
        // _cellPosition : 시작 위치, targetCell : 목표 위치
        List<Vector3Int> path = _pathfinding.FindPath(_cellPosition, targetCell);

        if (path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
        }
        
        
    }

    /// <summary>
    /// ī�޶� ����/�׸��� ��鿡 ���� ���콺�� ���� '�׸��� ���'���� ���� ��ǥ�� ��´�.
    /// </summary>
    private bool TryGetMouseWorldOnGrid(out Vector3 world)
    {
        var mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

        //XZ로 설정
        Plane plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            world = ray.GetPoint(enter);
            return true;
        }
        world = default;
        return false;
    }

    // 현재 셀 위치를 부르는 함수
    public Vector3Int GetCellPosition()
    {
        return _cellPosition;
    }

    // A* 알고리즘으로 찾은 경로를 따라 이동
    private IEnumerator FollowPath(List<Vector3Int> path)
    {
        foreach (var cell in path)
            yield return MoveRoutine(cell);
    }

    // 타겟 셀 위치로 부드럽게 이동(보정)
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