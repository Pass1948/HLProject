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
    [SerializeField] private int moveRange; // 이동 범위
    [SerializeField] private float moveTime = 0.2f;

    public Vector3Int _cellPosition; // 플레이어 현재 위치
    public bool _isMoving = false;  // 움직임 감지
    public bool isPlayer = false;

    private Pathfinding _pathfinding;

    private BasePlayer basePlayer;

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.PlayerMove, SwitchMove);
    }
    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.PlayerMove, SwitchMove);
    }
    private void Awake()
    {
        basePlayer = GetComponent<BasePlayer>();

    }

    private void Start()
    {
        tilemap = GameManager.Map.tilemap;
        // 플레이어 시작 위치를 타일의 중앙으로 설정
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);
        moveRange = basePlayer.playerModel.moveRange;

        // A* 알고리즘 초기화
        _pathfinding = new Pathfinding(tilemap);
    }
    private void Update()
    {
        GetCellPosition();
        //TODO: 마우스가 움직일 때마다 경로 미리보기(장보석,이영신)

        //if (isPlayer == true)
        //{
        //    if (TryGetMouseWorldOnGrid(out var mouseWorld))
        //    {
        //        var targetCell = tilemap.WorldToCell(mouseWorld);
        //        if (targetCell != _cellPosition)
        //        {
        //            var path = _pathfinding.FindPath(_cellPosition, targetCell);

        //            PlayerMoveRange(path, tilemap, moveRange);
        //        }
        //    }
        //}
    }


    public void PlayerMoveRange(List<Vector3Int> path, Tilemap tilemap, int moveRange)
    {
        GameManager.PathPreview.ShowPath(path, tilemap, 10);    // 맵크기는 나중에 데이터로 받아오는걸로 바꾸기
    }

    public void SwitchMove()
    {
        if (_isMoving == false)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }
    }

    public void GetPosition(int x, int y)
    {
        _cellPosition = new Vector3Int(x, y, 0);
    }

    private void OnMovementClick(InputValue value)
    {
        // UI 가 있어도 클릭 안되게
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        GameManager.PathPreview.ClearPath();

        // 마우스 눌렀다가 땠을 때에는 처리 하지 않음
        if (!value.isPressed) return;

        // 통합된 마우스 클릭 처리
        HandleMouseClick();
    }

    //  마우스 클릭 처리 - 3개 메서드를 하나로 통합[작성자:이영신]
    private void HandleMouseClick()
    {
        var mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0, QueryTriggerInteraction.Ignore))// RaycastHit의 최대거리 설정으로 무한정 오류를 방지 
        {
            string tag = hit.collider.gameObject.tag;
            // 1. 플레이어 클릭 처리
            if (tag == "Player")
            {
                HandlePlayerClick();
                return;
            }

            // 2. 적 클릭 처리
            if (tag == "Enemy")
            {
                HandleEnemyClick(hit);
                return;
            }

            // 3. 타일맵 클릭 처리 (이동)
            if (tag == "TileMap")
            {
                HandleTileMapClick(hit.point);
                return;
            }
            CancelSelection();
            return;
        }
        CancelSelection();
    }

    // 플레이어 클릭 처리
    private void HandlePlayerClick()
    {
        // 플레이어 클릭시 이동범위 확인할수있음
        GameManager.Map.PlayerUpdateRange(_cellPosition, moveRange);
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
        isPlayer = true;
    }

    // 적 클릭 처리
    private void HandleEnemyClick(RaycastHit hit)
    {
        // 적 클릭시 정보창
        var enemy = hit.collider.GetComponentInParent<BaseEnemy>();
        GameManager.UI.GetUI<EnemyInfoPopUpUI>().SetData(enemy.enemyModel.unitName, enemy.enemyModel.attri, enemy.enemyModel.rank);
        GameManager.UI.OpenUI<EnemyInfoPopUpUI>();

        // 적 클릭시 인덱스 확인 하는 코드
        /*        var enemies = GameManager.Unit.enemies;
        int idx = enemies.IndexOf(enemy);*/
    }


    // 타일맵 클릭 처리 (이동)
    private void HandleTileMapClick(Vector3 worldPoint)
    {
        // 이동 중이 아니면 무시
        if (_isMoving != true) return;
        
        // 플레이어가 선택되지 않았으면 무시
        if (isPlayer != true) return;
        
        // 이동 액션 선택
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Move);
        
        // 이동 처리
        OnclickInfo(worldPoint);
        CancelSelection();
    }

    public void OnclickInfo(Vector3 mouseWorld)
    {
        // 마우스 위치를 셀 위치로 변환
        var targetCell = tilemap.WorldToCell(mouseWorld);
        // 위치의 변화가 없거나 같으면 무시
        if (targetCell == _cellPosition) return;

        // A* 알고리즘 경로 설정
        // _cellPosition : 시작 위치, targetCell : 목표 위치
        List<Vector3Int> path = _pathfinding.FindPath(_cellPosition, targetCell);
        Debug.Log($"Path Count : {_cellPosition}");

        if (path == null||path.Count > moveRange)
        {
            CancelSelection();
            return;
        }
        StopAllCoroutines();
        StartCoroutine(FollowPath(path));

        isPlayer = false;
    }

    private void CancelSelection()
    {
        isPlayer = false;
        GameManager.Map.ClearPlayerRange();
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
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

        GameManager.Map.UpdateObjectPosition((int)start.x, (int)start.y, (int)_cellPosition.x, (int)_cellPosition.y, TileID.Player);
        Debug.Log($"이건 못참지 {GameManager.Map.GetPlayerPosition()}");

        _isMoving = false;
    }
}