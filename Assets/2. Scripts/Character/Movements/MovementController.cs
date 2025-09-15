using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class MovementController : MonoBehaviour
{
    public enum GridPlane { XY, XZ }

    [Header("Grid Settings")]
    [SerializeField] private GridPlane gridPlane = GridPlane.XZ; // 그리드 평면 설정
    [SerializeField] private Tilemap tilemap; // 이동 기준 타일맵
    [SerializeField] private float groundY = 0f; // 그리드 셀 크기

    [Header("Movement Settings")]
    [SerializeField] private float moveTime = 0.2f;

    public Vector3Int _cellPosition; // 플레이어 현재 위치
    public bool _isMoving = false;  // 움직임 감지

    private Pathfinding _pathfinding;

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }


    private void Start()
    {
        GameManager.Data.playerData.playerMoveData.PlayerPos = _cellPosition;
        // 플레이어 시작 위치를 타일의 중앙으로 설정
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);

        // A* 알고리즘 초기화
        _pathfinding = new Pathfinding(tilemap);
    }
    private void Update()
    {
        GetCellPosition();
        //TODO: 마우스가 움직일 때마다 경로 미리보기(장보석,이영신)
        //if (_isMoving == false)return;
        if (TryGetMouseWorldOnGrid(out var mouseWorld))
        {
            var targetCell = tilemap.WorldToCell(mouseWorld);
            if (targetCell != _cellPosition)
            {
                var path = _pathfinding.FindPath(_cellPosition, targetCell);
                int moveRange = GameManager.Data.playerData.playerMoveData.MoveRange;
                PlayerMoveRange(path, tilemap, moveRange);
            }
        }

    }

    public void Init(Tilemap tilemap)
    {
        this.tilemap = tilemap;
        // 플레이어 시작 위치를 타일의 중앙으로 설정
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);

        // A* 알고리즘 초기화
        _pathfinding = new Pathfinding(tilemap);
    }


    public void PlayerMoveRange(List<Vector3Int> path, Tilemap tilemap, int moveRange)
    {
        GameManager.PathPreview.ShowPath(path, tilemap, moveRange);
    }



    private void OnMovementClick(InputValue value)
    {
        // UI 가 있어도 클릭 안되게
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        GameManager.PathPreview.ClearPath();

        // 마우스 눌렀다가 땠을 때에는 처리 하지 않음
        if (!value.isPressed) return;
        //if (TryGetMouseWorldOnPlayer()==true) // 플레이어 클릭시 bool값이 true일 경수 움직임 진행
        
            if (!TryGetMouseWorldOnGrid(out var mouseWorld)) return;
            // 마우스 위치를 셀 위치로 변환
            OnclickInfo(mouseWorld);
        
    }

    public void OnclickInfo(Vector3 mouseWorld)
    {
        var targetCell = tilemap.WorldToCell(mouseWorld);

        // 위치의 변화가 없거나 같으면 무시
        if (targetCell == _cellPosition) return;

        // A* 알고리즘 경로 설정
        // _cellPosition : 시작 위치, targetCell : 목표 위치
        List<Vector3Int> path = _pathfinding.FindPath(_cellPosition, targetCell);

        int moveRange = GameManager.Data.playerData.playerMoveData.MoveRange;

        if (path.Count > moveRange)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FollowPath(path));
    }

    /// <summary>
    /// 마우스 위치, 그리드 평면에 따른 월드 좌표 변환
    /// </summary>
    private bool TryGetMouseWorldOnGrid(out Vector3 world)
    {
        var mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // 레이를 쏴서 테그가 맵이 아니면 무시
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("TileMap"))
            {
                world = hit.point;
                return true;
            }
            else
            {
                world = default;
                return false;
            }
        }
        world = default;
        return false;
    }

    // 플레이어 클릭시 나오는 행동 메서드(작성자: 이영신)
    private bool TryGetMouseWorldOnPlayer()
    {
        var mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        // 레이를 쏴서 테그가 맵이 아니면 무시
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                // TODO: 플레이어 클릭시 이동범위 확인할수있음
                
                return true;
            }
            else
            {
                // TODO: 플레이어 아닌걸 클릭시 이동범위 사라짐
                return false;
            }
        }
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
        GameManager.Data.playerData.playerMoveData.PlayerPos = _cellPosition;
        _isMoving = false;
    }
}