using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
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

    // ===== 커서 추적 입력/좌표 변환 =====
    [Header("Raycast / Plane")]
    public bool useLayerRaycast = true;
    public LayerMask groundMask = ~0;                 // Ground/TileMap만 포함 권장
    public float maxRayDistance = 1000f;
    public bool usePlaneIfMiss = true;
    public float groundY = 0f;                        // 평면 교차 높이

    // ===== 경계/가용범위 등 =====
    [Header("Constraints")]
    public bool blockWhenUI = true;                   // UI 위 클릭/추적 무시
    public bool clampToBounds = true;                 // 맵 밖이면 모서리로 클램프
    public bool freezeWhenOutside = false;            // 맵 밖이면 정지(클램프 대신)
    public bool restrictPointerToMovable = false;     // 포인터가 Terrain만 허용(색 반영)
    public bool restrictMoveToRange = true;           // 경로 길이가 이동범위 초과면 막기

    // ===== 유닛 탐색 보강(선택) =====
    [Header("Instance Lookup (Optional)")]
    public bool useOverlapLookup = true;              // 인스턴스 보강 탐색
    public LayerMask unitDetectMask = ~0;             // Enemy/Player 포함
    [Range(0.1f, 1f)] public float overlapShrink = 0.9f;
    public float overlapHeight = 2f;

    // ===== 이동/선택 상태 =====
    [Header("Movement")]
    public float stepMoveTime = 0.2f;                 // 한 칸 보간 시간
    private bool isMoving = false;
    [SerializeField] private bool movePhaseActive = false;  // PlayerMove 페이즈에서만 true
    private BasePlayer selectedPlayer;               // 현재 선택된 플레이어
    private Vector3Int _selectedPlayerCell;
    private int selectedMoveRange;

    // ===== 내부 캐시 =====
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Vector3Int lastValidCell;


    // ===== 셀 → 유닛 인덱스(Overlap/FindObjectsOfType 대체) =====
    private readonly Dictionary<Vector3Int, BasePlayer> playersByCell = new();
    private readonly Dictionary<Vector3Int, BaseEnemy> enemiesByCell = new();



    private readonly List<GameObject> activeTiles = new List<GameObject>();

    public void CreateMouse() // Scene넘어갔을때 실행
    {
        var go = GameManager.Resource.Create<GameObject>(pointerResourcePath);
        go.transform.SetParent(this.transform);
        pointer = go.transform;
    }

    public void SetMouseVar() // MouseFollower클래스에 Awake()안에서 호출
    {
        map = GameManager.Map;
        tilemap = map.tilemap;
        cam = Camera.main;
        // 포인터를 살짝 띄워 Z-fighting 방지(오브젝트가 겹치는 현상)
        var p = pointer.position; p.y = groundY + 0.01f;
        pointer.position = p;
        RebuildUnitIndex();
    }

    public void MovingMouse()   // MouseFollower클래스에 LateUpdate()안에서 호출
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

    //  PlayerTurnState에서 호출
    public void ToggleMovePhase() => movePhaseActive = !movePhaseActive;

    // =====================================================================
    // 포인터 추적 & 셀 스냅
    // =====================================================================
    private bool UpdatePointer()
    {
        if (pointer == null || tilemap == null || map == null) return false;

        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return false;

        if (!TryGetMouseWorld(out var world)) return false;

        // 월드→셀
        Vector3Int cell = tilemap.WorldToCell(world);

        // 경계 처리
        bool inside = IsInside(cell);
        if (!inside)
        {
            if (freezeWhenOutside) return false;
            if (clampToBounds) { cell = ClampCell(cell); inside = true; }
        }

        // Terrain만 허용(옵션)
        bool allowed = true;
        if (restrictPointerToMovable && inside)
            allowed = map.IsMovable(cell);

        // “한 칸씩” 스냅
        if (allowed && cell != lastCell)
        {
            Vector3 center = tilemap.GetCellCenterWorld(cell);
            center.y = groundY + 0.01f;
            pointer.position = center;

            lastCell = cell;
            lastValidCell = cell;
        }

        return true;
    }

    // =====================================================================
    // 클릭 처리 (TileID 기반)
    // =====================================================================
    private void HandleLeftClick()
    {
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        var cell = GetCurrentCell();
        if (!IsInside(cell)) return;

        if (map.IsPlayer(cell))
        {
            OnClickPlayer(cell);
            return;
        }
        if (map.IsEnemy(cell))
        {
            OnClickEnemy(cell);
            return;
        }
        if (map.IsMovable(cell)) // Terrain
        {
            OnClickTerrain(cell);
            return;
        }

        // 그 외(Obstacle 등)
        CancelSelection();
    }

    // ===== 클릭 동작별 핸들러 =====

    // Player 셀 클릭 → 선택 + 이동범위 표시 (딕셔너리 사용)
    private void OnClickPlayer(Vector3Int cell)
    {
        if (isMoving) return;

        // 1) 딕셔너리 조회
        playersByCell.TryGetValue(cell, out selectedPlayer);

        // 2) 폴백: 단일 플레이어
        if (selectedPlayer == null && GameManager.Unit != null && GameManager.Unit.Player != null)
        {
            if (map.IsPlayer(cell))
                selectedPlayer = GameManager.Unit.Player;
        }

        // 3) 선택 셀/범위
        _selectedPlayerCell = cell;

        int modelRange = (selectedPlayer != null && selectedPlayer.playerModel != null) ? GameManager.Unit.Player.playerModel.moveRange : -1;
        Debug.Log($"초기 이동 범위 : {selectedPlayer.playerModel.moveRange} 지금 이동범위: {modelRange}");
        // moveRange가 0/음수/미설정이면 map.moveRange로 폴백
        selectedMoveRange = (modelRange > 0) ? modelRange : map.moveRange;
        
        // 4) 범위 표시
        map.PlayerUpdateRange(cell, GameManager.Unit.Player.playerModel.moveRange);
        Debug.Log($"초기 이동 범위 : {GameManager.Unit.Player.playerModel.moveRange} ");
        GameManager.UI.OpenUI<MainUI>();
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
    }


    // Enemy 셀 클릭 → 정보 UI (딕셔너리 사용)
    private void OnClickEnemy(Vector3Int cell)
    {
        if (isMoving) return;

        if (enemiesByCell.TryGetValue(cell, out var enemy) && enemy != null)
        {
            GameManager.UI.GetUI<EnemyInfoPopUpUI>().SetData(enemy.enemyModel.unitName, enemy.enemyModel.attri, enemy.enemyModel.rank);
            GameManager.UI.OpenUI<EnemyInfoPopUpUI>();
        }
        else
        {
            // 좌표-인스턴스 매칭 실패 시 선택 해제
            CancelSelection();
        }
    }

    // Terrain 셀 클릭 → 선택된 플레이어가 있으면 이동 시도
    private void OnClickTerrain(Vector3Int destCell)
    {
        GameManager.UI.CloseUI<MainUI>();

        // 0) 이동 페이즈가 아니면 무시 (PlayerMove 단계에서만 허용)
        if (movePhaseActive == false) return;

        // 1) 현재 이동 중이면 무시
        if (isMoving) return;

        // 2) 플레이어가 선택되어 있어야 함
        if (selectedPlayer == null) return;

        // 3) 같은 칸이면 무시
        if (destCell == _selectedPlayerCell) return;

        // 4) 이동 액션 선택(턴제 시스템 연동은 기존 흐름 유지)
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Move);

        // 5) 경로 계산
        List<Vector3Int> path = map.FindPath(_selectedPlayerCell, destCell);

        // 6) 경로/범위 검증
        if (path == null || path.Count == 0)
        {
            CancelSelection();
            return;
        }
        int steps = path.Count - 1;
        if (restrictMoveToRange && steps > selectedMoveRange)
        {
            CancelSelection();
            return;
        }

        // 7) 이동 시작
        StopAllCoroutines();
        StartCoroutine(MoveAlongPath(selectedPlayer.transform, _selectedPlayerCell, path, TileID.Player));

        // 8) 추가 입력 차단 + 선택/범위 UI 정리
        movePhaseActive = false;
        CancelSelection();
    }

    // ===== 이동 실행(한 칸씩 보간 + 맵데이터 갱신 + 인덱스 갱신) =====
    private IEnumerator MoveAlongPath(Transform actor, Vector3Int currentCell, List<Vector3Int> path, int tileIdForActor)
    {
        isMoving = true;

        foreach (var nextCell in path)
        {
            Vector3 start = actor.position;
            Vector3 end = tilemap.GetCellCenterWorld(nextCell);

            float t = 0f;
            float dur = Mathf.Max(0.0001f, stepMoveTime);
            while (t < 1f)
            {
                t += Time.deltaTime / dur;
                actor.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
            actor.position = end;

            // 맵데이터 갱신(셀 기준)
            map.UpdateObjectPosition(currentCell.x, currentCell.y, nextCell.x, nextCell.y, tileIdForActor);

            // 인덱스(셀→유닛) 갱신: 플레이어만 이동한다고 가정
            UpdateUnitIndexOnMove(currentCell, nextCell);

            // 내부 상태 갱신
            currentCell = nextCell;
            _selectedPlayerCell = nextCell;
        }

        isMoving = false;
    }

    // ===== 유닛 인덱스 관리 =====

    //씬/스폰 완료 후 전체 인덱스를 재구축합니다.
    public void RebuildUnitIndex()
    {
        playersByCell.Clear();
        enemiesByCell.Clear();

        // 플레이어 인덱스 구축
        var player = (GameManager.Unit != null) ? GameManager.Unit.Player : null;
        if (player != null)
        {
            var cell = tilemap.WorldToCell(player.transform.position);
            playersByCell[cell] = player;
        }

        // 적 인덱스 구축
        if (GameManager.Unit != null && GameManager.Unit.enemies != null)
        {
            foreach (var e in GameManager.Unit.enemies)
            {
                if (!e) continue;
                var cell = tilemap.WorldToCell(e.transform.position);
                enemiesByCell[cell] = e;
            }
        }
    }

    //플레이어 이동 시 셀 인덱스를 즉시 갱신합니다.
    private void UpdateUnitIndexOnMove(Vector3Int oldCell, Vector3Int newCell)
    {
        if (selectedPlayer == null) return;
        playersByCell.Remove(oldCell);
        playersByCell[newCell] = selectedPlayer;
    }
    // ===== 공용 유틸 =====
    public Vector3Int GetCurrentCell(bool preferValid = true)
        => preferValid ? (lastValidCell == default ? lastCell : lastValidCell) : lastCell;

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

    private bool IsInside(Vector3Int c) => c.x >= 0 && c.y >= 0 && c.x < map.mapWidth && c.y < map.mapHeight;

    private Vector3Int ClampCell(Vector3Int c)
    {
        int x = Mathf.Clamp(c.x, 0, map.mapWidth - 1);
        int y = Mathf.Clamp(c.y, 0, map.mapHeight - 1);
        return new Vector3Int(x, y, 0);
    }

    // 선택 해제 + UI/범위 정리
    private void CancelSelection()
    {
        selectedPlayer = null;
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
