using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Vector3Int PointerCell => tilemap.WorldToCell(pointer.position);
    
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
    public bool movePhaseActive = false;  // PlayerMove 페이즈에서만 true
    private BasePlayer selectedPlayer;               // 현재 선택된 플레이어
    private Vector3Int selectedPlayerCell;
    private int selectedMoveRange;
    bool isPlayer = false;
    
    
    //==== 공격 상태 =====
    private bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }


    //==== 발차기 상태 =====
    private bool isKicking = false;
    public bool IsKicking { get { return isKicking; } set { isKicking = value; } }

    // ===== 내부 캐시 =====
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Vector3Int lastValidCell;

    // --- 마우스 토글 상태 ---
    private bool playerRangeVisible = false;
    private BaseEnemy selectedEnemy;       // 현재 팝업이 열린 적
    private bool enemyPopupVisible = false;
    public bool isMouse = false;
    public bool isShowRange = false;

    // --- Overlap관련 최적화 ---
    private readonly Collider[] oneHit = new Collider[1];// OverlapBoxNonAlloc 결과 담는 1칸
    private InputAction _point;            // <Pointer>/position
    private InputAction _click;            // <Pointer>/press
    private Vector3Int _hoverCell;         // 미리 입력된(호버 중) 셀
    private bool _hasHover;                // 호버 유효 여부
    private void OnEnable()
    {
        // Pointer Position
        _point = new InputAction("Point", binding: "<Pointer>/position");
        _point.Enable();

        // Click(press)
        _click = new InputAction("Click", binding: "<Pointer>/press");
        _click.performed += OnClickPerformed;   // ✅ 클릭 순간에 현재 호버 셀로 확정
        _click.Enable();
    }

    public void CreateMouse() // Scene넘어갔을때 실행
    {
        var go = GameManager.Resource.Create<GameObject>(pointerResourcePath);
        go.transform.SetParent(this.transform);
        pointer = go.transform;
    }

    public void SetMouseVar() 
    {
        map = GameManager.Map;
        tilemap = map.tilemap;
        cam = Camera.main;
    }

    public void MovingMouse()   // MouseFollower클래스에 LateUpdate()안에서 호출
    {
        UpdatePointerFromInput();
    }
    
    //  PlayerTurnState에서 호출
    public void SetMovePhase(bool active) => movePhaseActive = active;

    // =====================================================================
    // 포인터 추적 & 셀 스냅
    // =====================================================================
    private void UpdatePointerFromInput()
    {
        if (pointer == null || tilemap == null || map == null || cam == null) return;

        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        {
            _hasHover = false;
            return;
        }

        // 1) 화면 좌표 읽기
        Vector2 screen = _point != null ? _point.ReadValue<Vector2>() : Vector2.zero;

        // 2) 화면→월드
        if (!TryGetMouseWorld(screen, out var world))
        {
            _hasHover = false;
            return;
        }

        // 3) 월드→셀
        Vector3Int cell = tilemap.WorldToCell(world);

        // 4) 경계 처리
        bool inside = IsInside(cell);
        if (!inside)
        {
            if (freezeWhenOutside)
            {
                _hasHover = false;
                return;
            }
            if (clampToBounds)
            {
                cell = ClampCell(cell);
                inside = true;
            }
        }

        // 5) Terrain만 허용(옵션)
        bool allowed = true;
        if (restrictPointerToMovable && inside)
            allowed = map.IsMovable(cell);

        // 6) 포인터 스냅 + 호버셀 갱신
        if (allowed && cell != lastCell)
        {
            Vector3 center = tilemap.GetCellCenterWorld(cell);
            center.y = groundY + 0.01f;
            pointer.position = center;

            lastCell = cell;
            lastValidCell = cell;
        }

        _hoverCell = cell;
        _hasHover = allowed && inside;
    }

    // =====================================================================
    // 클릭 처리 (TileID 기반)
    // =====================================================================

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        if (!_hasHover) return;                       // 호버가 유효할 때만 클릭 처리
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        // 클릭 시점에 “미리 입력된 셀(_hoverCell)”을 확정하여 처리
        HandleLeftClick(_hoverCell);
    }

    private void HandleLeftClick(Vector3Int cell)
    {
        if (!IsInside(cell)) { CancelSelection(); return; }

        // 셀 분류 결과를 1회만 계산 (중복 호출 제거)
        bool cellIsPlayer = map.IsPlayer(cell);
        bool cellIsEnemy = map.IsEnemy(cell);
        bool cellIsTerrain = map.IsMovable(cell); // Terrain
        Debug.Log($"지금 자리는 Player {cellIsPlayer}");
        Debug.Log($"지금 자리는 Enemy {cellIsEnemy}");
        Debug.Log($"지금 자리는 Terrain {cellIsTerrain}");
        // 공격 모드 우선
        if (isAttacking)
        {
            if (IsCellInAttackOrKickRange(cell))
            {
                GameManager.Event.Publish(EventType.PlayerAttack); // 공격 State에서 일괄 처리
                isAttacking = false;

            }
            else
            {
                // TODO : 범위 밖 클릭 → 실행 안 함 + 범위 끄기(기획자들과 상의후 설정)
                CancelAttackOrKickRange();
                isAttacking = false;
            }
            return;
        }

        // 킥 모드 우선
        if (isKicking)
        {
            // 적이거나 지면일 때 킥 상태로 전환
            if (IsCellInAttackOrKickRange(cell))
            {
                GameManager.TurnBased.ChangeTo<PlayerKickState>();
                isKicking = false;
                // 킥 범위는 상태 진입 시/후 정리 루틴에 맡김
            }
            else
            {
                // TODO : 범위 밖 클릭 → 실행 안 함 + 범위 끄기 (기획자들과 상의후 설정)
                CancelAttackOrKickRange();
                isKicking = false;
            }
            return;
        }

        // 일반 모드
        if (cellIsPlayer)
        {
            isPlayer = true;
            OnClickPlayer(cell);
            return;
        }

        if (cellIsEnemy)
        {
            isPlayer = false;
            OnClickEnemy(cell);
            return;
        }

        if (cellIsTerrain)
        {
            OnClickTerrain(cell);
            return;
        }

        // 기타(Obstacle 등)
        isPlayer = false;
        CancelSelection();
    }



    // ===== 클릭 동작별 핸들러 =====

    // Player 셀 클릭 => 선택 + 이동범위 표시
   private void OnClickPlayer(Vector3Int cell)
    {
        // 공격/킥 모드면: 범위만 끄고 취소
        if (isAttacking || IsKicking)
        {
            CancelAttackOrKickRange();
            isAttacking = false;
            IsKicking = false;
            HidePlayerRange();
            return;
        }

        HideEnemyPopup();
        if (isMoving) return;

        selectedPlayer      = useOverlapLookup ? FindAtCell<BasePlayer>(cell) : null;
        selectedPlayerCell  = cell;
        selectedMoveRange   = GameManager.Unit.Player.playerModel.moveRange;

        if (!isShowRange) return;

        if (playerRangeVisible && selectedPlayerCell == cell)
            HidePlayerRange();
        else
            ShowPlayerRange(cell);
    }

    private void OnClickEnemy(Vector3Int cell)
    {
        HidePlayerRange();
        if (isMoving) return;

        var enemy = useOverlapLookup ? FindAtCell<BaseEnemy>(cell) : null;
        if (enemy == null)
        {
            HideEnemyPopup();
            CancelSelection();
            return;
        }

        if (enemyPopupVisible && selectedEnemy == enemy)
            HideEnemyPopup();
        else
            ShowEnemyPopup(enemy);
    }

    private void OnClickTerrain(Vector3Int destCell)
    {
        map.ClearPlayerRange();
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();

        if (!isPlayer) return;
        if (movePhaseActive == false) return;              // PlayerMove 페이즈만 허용
        if (selectedPlayer == null) return;
        if (destCell == selectedPlayerCell) return;

        var path = map.FindPath(selectedPlayerCell, destCell);
        if (path == null || path.Count > selectedMoveRange)
        {
            CancelSelection();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(MoveAlongPath(selectedPlayer.transform, selectedPlayerCell, path, TileID.Player));
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Move);
        movePhaseActive = false;
        CancelSelection();
    }

    // ===== 이동 실행(한 칸씩 보간 + 맵데이터 갱신) =====
    public IEnumerator MoveAlongPath(Transform actor, Vector3Int currentCell, List<Vector3Int> path, int tileIdForActor)
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
            map.UpdateObjectPosition(currentCell, currentCell, nextCell, nextCell, tileIdForActor);

            currentCell = nextCell;
            selectedPlayerCell = nextCell;
        }

        isMoving = false;
    }

    // =====================================================================
    // 공용/유틸
    // =====================================================================
    private bool TryGetMouseWorld(Vector2 screen, out Vector3 world)
    {
        world = default;
        var ray = cam.ScreenPointToRay(screen);

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

    private void CancelSelection()
    {
        selectedPlayer = null;
        map.ClearPlayerRange();
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
    }

    public T FindAtCell<T>(Vector3Int cell) where T : Component
    {
        if (!useOverlapLookup || tilemap == null) return null;

        Vector3 center = tilemap.GetCellCenterWorld(cell);
        Vector3 halfExtents = new Vector3(
            tilemap.cellSize.x * 0.5f * overlapShrink,
            overlapHeight * 0.5f,
            tilemap.cellSize.y * 0.5f * overlapShrink
        );

        int hitCount = Physics.OverlapBoxNonAlloc(
            center,
            halfExtents,
            oneHit,
            Quaternion.identity,
            unitDetectMask,
            QueryTriggerInteraction.Ignore
        );

        if (hitCount <= 0) return null;
        var col = oneHit[0];
        if (!col) return null;

        if (col.TryGetComponent<T>(out var direct))
            return direct;

        return col.GetComponentInParent<T>(true);
    }

    private void ShowPlayerRange(Vector3Int cell)
    {
        playerRangeVisible = true;
        map.ClearPlayerRange();
        map.PlayerUpdateRange(cell, selectedMoveRange);
    }
    private void HidePlayerRange()
    {
        playerRangeVisible = false;
        map.ClearPlayerRange();
    }

    private void ShowEnemyPopup(BaseEnemy enemy)
    {
        if (enemy == null) return;
        selectedEnemy = enemy;
        enemyPopupVisible = true;
        GameManager.UI.GetUI<EnemyInfoPopUpUI>()
            .SetData(enemy.enemyModel.attri, enemy.enemyModel.rank, enemy.enemyModel.attack, enemy.enemyModel.moveRange, enemy.enemyModel.currentHealth, enemy.enemyModel.maxHealth);
        GameManager.UI.OpenUI<EnemyInfoPopUpUI>();
    }
    private void HideEnemyPopup()
    {
        enemyPopupVisible = false;
        selectedEnemy = null;
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
    }

    private bool IsCellInAttackOrKickRange(Vector3Int cell)
    {
        return map != null
               && map.attackRangeTilemap != null
               && map.attackRangeTilemap.HasTile(cell);
    }
    private void CancelAttackOrKickRange()
    {
        if (GameManager.Map != null && GameManager.Map.attackRange != null)
            GameManager.Map.attackRange.ClearAttackType();
    }
}
