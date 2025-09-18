using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    // ===== 레퍼런스 =====
    [Header("References")]
    [SerializeField] public MapManager map;      // 비우면 GameManager.Map
    [SerializeField] public Tilemap tilemap;     // 비우면 map.tilemap
    [SerializeField] public Camera cam;          // 비우면 Camera.main
    [SerializeField] public Transform pointer;   // 포인터(커서 비주얼). 비우면 생성

    [Header("Pointer Resource (선택)")]
    [Tooltip("GameManager.Resource 경로를 쓰는 경우에만 사용. 비워도 됨.")]
    public string pointerResourcePath = Path.Mouse + "Pointer";
    [Tooltip("포인터를 그리드 밑에 붙이고 싶다면 체크")]
    public bool parentPointerUnderGrid = true;

    // ===== 커서 추적 입력/좌표 변환 =====
    [Header("Raycast / Plane")]
    public bool useLayerRaycast = true;
    public LayerMask groundMask = ~0;                 // Ground/TileMap만 포함 권장
    public float maxRayDistance = 1000f;
    public bool usePlaneIfMiss = true;
    public float groundY = 0f;                        // 평면 교차 높이

    // ===== 경계/가용성 정책 =====
    [Header("Constraints")]
    public bool blockWhenUI = true;                   // UI 위 클릭/추적 무시
    public bool clampToBounds = true;                 // 맵 밖이면 모서리로 클램프
    public bool freezeWhenOutside = false;            // 맵 밖이면 정지(클램프 대신)
    public bool restrictPointerToMovable = false;     // 포인터가 Terrain만 허용(색 반영)
    public bool restrictMoveToRange = true;           // 경로 길이가 이동범위 초과면 막기

    // ===== 포인터 비주얼 =====
    [Header("Visual")]
    public bool tintBlocked = true;
    public Color movableColor = Color.white;
    public Color blockedColor = new Color(1, 0.5f, 0.5f);
    private Renderer _pointerRenderer;

    // ===== 유닛 탐색 보강(선택) =====
    [Header("Instance Lookup (Optional)")]
    public bool useOverlapLookup = true;              // 인스턴스 보강 탐색
    public LayerMask unitDetectMask = ~0;             // Enemy/Player 포함
    [Range(0.1f, 1f)] public float overlapShrink = 0.9f;
    public float overlapHeight = 2f;

    // ===== 이동/선택 상태 =====
    [Header("Movement")]
    public float stepMoveTime = 0.2f;                 // 한 칸 보간 시간
    private bool _isMoving = false;

    private BasePlayer _selectedPlayer;               // 현재 선택된 플레이어
    private Vector3Int _selectedPlayerCell;
    private int _selectedMoveRange;

    // ===== 내부 캐시 =====
    private Vector3Int _lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Vector3Int _lastValidCell;

    private readonly List<GameObject> activeTiles = new List<GameObject>();
    // ===== 라이프사이클 =====
    private void Awake()
    {
        if (!map) map = GameManager.Map;
        if (!tilemap && map) tilemap = map.tilemap;
        if (!cam) cam = Camera.main;

        // 포인터 준비
        if (!pointer)
        {
            // 리소스 매니저가 있다면 프리팹 생성
            if (!string.IsNullOrEmpty(pointerResourcePath))
            {
                var go = GameManager.Resource.Create<GameObject>(pointerResourcePath);
                pointer = go.transform;
            }
            else
            {
                // 임시 쿼드 생성
                var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = "MousePointer";
                pointer = go.transform;
            }
        }
        // 포인터 부모 정리(그리드 밑)
        if (parentPointerUnderGrid && tilemap)
            pointer.SetParent(tilemap.transform, worldPositionStays: true);

        _pointerRenderer = pointer.GetComponentInChildren<Renderer>();
        if (!_pointerRenderer) _pointerRenderer = pointer.GetComponent<Renderer>();

        // 포인터를 살짝 띄워 Z-fighting 방지
        var p = pointer.position; p.y = groundY + 0.01f; pointer.position = p;
    }

    private void Update()
    {
        // 1) 포인터 추적(셀 단위)
        if (!UpdatePointer()) return;

        // 2) 좌클릭 라우팅 (입력)
        bool clicked =
#if ENABLE_INPUT_SYSTEM
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
            Input.GetMouseButtonDown(0);
#endif
        if (clicked) HandleLeftClick();
    }

    // ===== 포인터 추적 & 셀 스냅 =====
    private bool UpdatePointer()
    {
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return false;

        if (!TryGetMouseWorld(out var world)) return false;

        // 월드→셀
        Vector3Int cell = tilemap.WorldToCell(world);

        // 경계 처리
        bool inside = IsInside(cell);
        if (!inside)
        {
            if (freezeWhenOutside) { ApplyTint(_lastValidCell, allowed: true); return false; }
            if (clampToBounds) { cell = ClampCell(cell); inside = true; }
        }

        // Terrain만 허용(옵션)
        bool allowed = true;
        if (restrictPointerToMovable && inside)
            allowed = map.IsMovable(cell);

        // 셀 변경시에만 스냅 → "한 칸씩" 움직임
        if (allowed && cell != _lastCell)
        {
            GridSnapper.SnapToCellCenter(pointer, tilemap, new Vector2Int(cell.x, cell.y));
            _lastCell = cell;
            _lastValidCell = cell;
        }

        ApplyTint(cell, allowed);
        return true;
    }

    // 좌클릭 처리(셀 기반 분기)
    private void HandleLeftClick()
    {
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        var cell = GetCurrentCell();
        if (!IsInside(cell)) return;

        int id = map.mapData[cell.x, cell.y];

        switch (id)
        {
            case TileID.Player:
                OnClickPlayer(cell);
                break;

            case TileID.Enemy:
                OnClickEnemy(cell);
                break;

            case TileID.Terrain:
                OnClickTerrain(cell);
                break;

            default:
                CancelSelection();
                break;
        }
    }

    // ===== 클릭 동작별 핸들러 =====

    // Player 셀 클릭 → 선택 + 이동범위 표시
    private void OnClickPlayer(Vector3Int cell)
    {
        if (_isMoving) return;

        // 셀에서 실제 플레이어 컴포넌트 탐색(보강)
        _selectedPlayer = useOverlapLookup ? FindAtCell<BasePlayer>(cell) : null;
        _selectedPlayerCell = cell;

        // 이동 범위 설정(플레이어 모델이 있으면 거기서, 없으면 MapManager의 기본값 사용)
        _selectedMoveRange = (_selectedPlayer && _selectedPlayer.playerModel != null)
            ? _selectedPlayer.playerModel.moveRange
            : map.moveRange;

        // UI/범위 표시
        map.PlayerUpdateRange(cell, _selectedMoveRange);
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
    }

    // Enemy 셀 클릭 → 정보 UI
    private void OnClickEnemy(Vector3Int cell)
    {
        if (_isMoving) return;

        var enemy = useOverlapLookup ? FindAtCell<BaseEnemy>(cell) : null;
        if (enemy != null)
        {
            GameManager.UI.GetUI<EnemyInfoPopUpUI>()
                .SetData(enemy.enemyModel.unitName, enemy.enemyModel.attri, enemy.enemyModel.rank);
            GameManager.UI.OpenUI<EnemyInfoPopUpUI>();
        }
        else
        {
            // 인스턴스가 없으면 취소(선택 해제)
            CancelSelection();
        }
    }

    // Terrain 셀 클릭 → 선택된 플레이어가 있으면 이동 시도
    private void OnClickTerrain(Vector3Int destCell)
    {
        if (_isMoving) return;
        if (_selectedPlayer == null) return;

        // 선택된 플레이어의 현재 위치(셀)
        Vector3Int startCell = _selectedPlayerCell;

        // 경로 계산
        List<Vector3Int> path = map.FindPath(startCell, destCell);
        if (path == null || path.Count == 0)
        {
            CancelSelection(); return;
        }

        // 범위 제한
        if (restrictMoveToRange && path.Count > _selectedMoveRange)
        {
            // 초과 시 이동 불가 (원하면 path를 잘라서 N칸까지만 이동하도록 바꿀 수 있음)
            CancelSelection(); return;
        }

        // 이동 액션 선택(턴제 시스템 연동)
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Move);

        // 이동 코루틴 시작
        StopAllCoroutines();
        StartCoroutine(MoveAlongPath(_selectedPlayer.transform, startCell, path, TileID.Player));
    }

    // ===== 이동 실행(한 칸씩 보간 + 맵데이터 갱신) =====
    private IEnumerator MoveAlongPath(Transform actor, Vector3Int currentCell, List<Vector3Int> path, int tileIdForActor)
    {
        _isMoving = true;

        foreach (var nextCell in path)
        {
            // 월드 좌표 보간
            Vector3 start = actor.position;
            Vector3 end = tilemap.GetCellCenterWorld(nextCell);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.0001f, stepMoveTime);
                actor.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
            actor.position = end;

            // 맵 데이터 갱신(셀 좌표 기반)
            map.UpdateObjectPosition(currentCell.x, currentCell.y, nextCell.x, nextCell.y, tileIdForActor);

            // 내부 상태 갱신
            currentCell = nextCell;
            _selectedPlayerCell = nextCell; // 선택 유지 중이면 최신 위치로
        }

        _isMoving = false;
        // 이동 완료 후 선택/범위 해제(원한다면 유지하도록 옵션화 가능)
        CancelSelection();
    }

    // ===== 공용 유틸 =====
    public Vector3Int GetCurrentCell(bool preferValid = true)
        => preferValid ? (_lastValidCell == default ? _lastCell : _lastValidCell) : _lastCell;

    private bool TryGetMouseWorld(out Vector3 world)
    {
        world = default;
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null) return false;
        Vector2 mousePos = Mouse.current.position.ReadValue();
#else
        Vector2 mousePos = Input.mousePosition;
#endif
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (useLayerRaycast &&
            Physics.Raycast(ray, out var hit, maxRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        { world = hit.point; return true; }

        if (usePlaneIfMiss)
        {
            var plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
            if (plane.Raycast(ray, out float enter))
            { world = ray.GetPoint(enter); return true; }
        }
        return false;
    }

    private bool IsInside(Vector3Int c)
        => c.x >= 0 && c.y >= 0 && c.x < map.mapWidth && c.y < map.mapHeight;

    private Vector3Int ClampCell(Vector3Int c)
    {
        int x = Mathf.Clamp(c.x, 0, map.mapWidth - 1);
        int y = Mathf.Clamp(c.y, 0, map.mapHeight - 1);
        return new Vector3Int(x, y, 0);
    }

    private void ApplyTint(Vector3Int cell, bool allowed)
    {
        if (!tintBlocked || _pointerRenderer == null) return;
        // 런타임 인스턴스만 수정하도록 .material 사용(공유 머티리얼 변조 방지)
        _pointerRenderer.material.color = allowed ? movableColor : blockedColor;
    }

    // 셀 중심 주변에서 컴포넌트 탐색(보강). MapManager에 셀→유닛 매핑이 있으면 그걸 쓰는 게 최선.
    private T FindAtCell<T>(Vector3Int cell) where T : Component
    {
        if (!useOverlapLookup) return null;

        Vector3 center = tilemap.GetCellCenterWorld(cell);
        Vector3 size = new Vector3(tilemap.cellSize.x * overlapShrink, overlapHeight, tilemap.cellSize.y * overlapShrink);
        var cols = Physics.OverlapBox(center, size * 0.5f, Quaternion.identity, unitDetectMask, QueryTriggerInteraction.Collide);
        foreach (var c in cols)
        {
            var t = c.GetComponentInParent<T>();
            if (t) return t;
        }
        return null;
    }

    // 선택 해제 + UI/범위 정리
    private void CancelSelection()
    {
        _selectedPlayer = null;
        map.ClearPlayerRange();
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
    }





















    //================= Pathfinding의 잔재 더이상 쓸모 없을때 지우기 =================//
    public void ShowPath(Vector3Int[] path, Tilemap tilemap, int moveRange, GameObject mouse)
    {
        ClearPath();

        // 이동 범위 제한
        int maxRange = Mathf.Min(path.Length, moveRange);
        for (int i = 0; i < path.Length; i++)
        {
            if (tilemap == null) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(path[i]);
            if (i < moveRange)
            {
                mouse.transform.position = worldPos;
            }
            else
                return;
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
