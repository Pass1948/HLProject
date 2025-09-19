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
    private Vector3Int selectedPlayerCell;
    private int selectedMoveRange;
    bool isPlayer = false;

    //==== 공격 상태 =====
    private bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    // ===== 내부 캐시 =====
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Vector3Int lastValidCell;

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
            isPlayer = true;
            OnClickPlayer(cell);
            return;
        }
        if (map.IsEnemy(cell) && isAttacking == false)
        {
            isPlayer = false;
            GameManager.UI.CloseUI<MainUI>();
            OnClickEnemy(cell);
            return;
        }
        else if (map.IsEnemy(cell) && isAttacking == true)
        {
            GameManager.TurnBased.SetSelectedAction(PlayerActionType.Attack);
            isAttacking = false;
        }

        if (map.IsMovable(cell)&& isAttacking==false) // Terrain
        {
            OnClickTerrain(cell);
            GameManager.UI.CloseUI<MainUI>();
            return;
        }
        else if(map.IsMovable(cell) && isAttacking==true)
        {
            GameManager.TurnBased.SetSelectedAction(PlayerActionType.Attack);
            isAttacking = false;
        }

        isPlayer = false;
        // 그 외(Obstacle 등)
        CancelSelection();
    }

    // ===== 클릭 동작별 핸들러 =====

    // Player 셀 클릭 => 선택 + 이동범위 표시
    private void OnClickPlayer(Vector3Int cell)
    {
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
        GameManager.UI.OpenUI<MainUI>();
        if (isMoving) return;
        // 셀에서 실제 플레이어 컴포넌트 탐색(보강)
        selectedPlayer = useOverlapLookup ? FindAtCell<BasePlayer>(cell) : null; selectedPlayerCell = cell;
        // 이동 범위 설정(플레이어 모델이 있으면 거기서, 없으면 MapManager의 기본값 사용)
        selectedMoveRange = GameManager.Unit.Player.playerModel.moveRange; // UI/범위 표시

        if (isAttacking == false)// 공격모드 일 경우 이동범위 표시 안함
        {
            map.ClearPlayerRange();
            map.PlayerUpdateRange(cell, selectedMoveRange);
        }
    }

    // Enemy 셀 클릭 => 정보 UI
    private void OnClickEnemy(Vector3Int cell)
    {
        map.ClearPlayerRange();
        if (isMoving) return;
        var enemy = useOverlapLookup ? FindAtCell<BaseEnemy>(cell) : null;
        // 공격범위 셀 id를 적과 비교해서 일치하면 공격
        if (enemy != null)
        {
            GameManager.UI.GetUI<EnemyInfoPopUpUI>().SetData(enemy.enemyModel.unitName, enemy.enemyModel.attri, enemy.enemyModel.rank);
            GameManager.UI.OpenUI<EnemyInfoPopUpUI>();
        }
        else
        { //인스턴스가 없으면 취소(선택 해제)
            CancelSelection();
        }

    }


    // Terrain 셀 클릭 → 선택된 플레이어가 있으면 이동 시도
    private void OnClickTerrain(Vector3Int destCell)
    {

            GameManager.Map.attackRange.ClearRange();
            if (isPlayer == true)
            {
                // 이동 페이즈가 아니면 무시 (PlayerMove 단계에서만 허용)
                if (movePhaseActive == false) return;

                //  현재 이동 중이면 무시
                //if (isMoving) return;

                //  플레이어가 선택되어 있어야 함
                if (selectedPlayer == null) return;

                //  같은 칸이면 무시
                if (destCell == selectedPlayerCell) return;

                // 경로 계산
                List<Vector3Int> path = map.FindPath(selectedPlayerCell, destCell);

                // 경로/범위 검증
                if (path == null || path.Count > selectedMoveRange)
                {
                    CancelSelection();
                    return;
                }

                // 이동 시작
                StopAllCoroutines();
                StartCoroutine(MoveAlongPath(selectedPlayer.transform, selectedPlayerCell, path, TileID.Player));
                GameManager.TurnBased.SetSelectedAction(PlayerActionType.Move);
                // 추가 입력 차단 + 선택/범위 UI 정리
                movePhaseActive = false;
                CancelSelection();
            }
        
    }

    // ===== 이동 실행(한 칸씩 보간 + 맵데이터 갱신) =====
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

            // 내부 상태 갱신
            currentCell = nextCell;
            selectedPlayerCell = nextCell;
        }

        isMoving = false;

    }





    // =====================================================================
    //공용 메서드
    // =====================================================================
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
    // 셀 중심 주변에서 컴포넌트 탐색(보강). MapManager에 셀→유닛 매핑이 있으면 그걸 쓰는 게 최선.
    private T FindAtCell<T>(Vector3Int cell) where T : Component
    {
        if (!useOverlapLookup) return null;
        Vector3 center = tilemap.GetCellCenterWorld(cell);
        Vector3 size = new Vector3(tilemap.cellSize.x * overlapShrink, overlapHeight, tilemap.cellSize.y * overlapShrink);
        var cols = Physics.OverlapBox(center, size * 0.5f, Quaternion.identity, unitDetectMask, QueryTriggerInteraction.Collide);
        foreach (var c in cols)
        {
            var t = c.GetComponentInParent<T>(); if (t) return t;
        }
        return null;
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
