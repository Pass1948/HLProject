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
    [SerializeField] private bool movePhaseActive = false;  // PlayerMove 페이즈에서만 true
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

    // --- Overlap관련 최적화 ---
    private readonly Collider[] oneHit = new Collider[1];// OverlapBoxNonAlloc 결과 담는 1칸
    

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
    }

    public void MovingMouse()   // MouseFollower클래스에 LateUpdate()안에서 호출
    {
        //포인터 추적(셀 단위)
        if (!UpdatePointer()) return;
        //if (isMouse == false) return;
        OnMouseClick();
    }
    
    public void OnSwitchIsClicked() =>isMouse = !isMouse;
    

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

        // Terrain만 허용
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

    public void OnMouseClick()
    {
       bool isClick =
#if ENABLE_INPUT_SYSTEM
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
            Input.GetMouseButtonDown(0);
#endif
        if (isClick) HandleLeftClick();
    }
    
    private void HandleLeftClick()
    {

        //공통 가드
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        var cell = GetCurrentCell();

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
                //CancelAttackOrKickRange();
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
                //CancelAttackOrKickRange();
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
        // 적 팝업은 닫아둠
        HideEnemyPopup();

        if (isMoving) return;

        // 선택/범위 기초값 세팅은 기존 그대로 유지
        selectedPlayer = useOverlapLookup ? FindAtCell<BasePlayer>(cell) : null;
        selectedPlayerCell = cell;
        selectedMoveRange = GameManager.Unit.Player.playerModel.moveRange;

        if (isAttacking) return;

            // 현재 범위가 떠 있고 같은 칸을 다시 눌렀다면 끄고, 아니면 켠다
            if (playerRangeVisible && selectedPlayerCell == cell)
                HidePlayerRange();
            else
                ShowPlayerRange(cell);
    }

    // Enemy 셀 클릭 => 정보 UI
    private void OnClickEnemy(Vector3Int cell)
    {
        // 플레이어 이동범위는 닫아둠
        HidePlayerRange();

        if (isMoving) return;

        var enemy = useOverlapLookup ? FindAtCell<BaseEnemy>(cell) : null;
        if (enemy == null)
        {
            // 해당 칸에서 적을 찾지 못하면 닫고 선택 해제
            HideEnemyPopup();
            CancelSelection();
            return;
        }

        // 같은 적을 다시 클릭하면 닫기, 아니면 새로 열기
        if (enemyPopupVisible && selectedEnemy == enemy)
            HideEnemyPopup();
        else
            ShowEnemyPopup(enemy);

    }


    // Terrain 셀 클릭 → 선택된 플레이어가 있으면 이동 시도
    private void OnClickTerrain(Vector3Int destCell)
    {
        map.ClearPlayerRange();
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
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
    // 셀 중심 주변에서 컴포넌트 탐색
    public T FindAtCell<T>(Vector3Int cell) where T : Component
    {
        if (!useOverlapLookup || tilemap == null) return null;  // 칸에 오브젝트가 없을경우 

        // 셀 중심과 halfExtents(반지름) 계산
        Vector3 center = tilemap.GetCellCenterWorld(cell);
        Vector3 halfExtents = new Vector3(
            tilemap.cellSize.x * 0.5f * overlapShrink,   // XZ 셀을 살짝 축소해 오검출 방지
            overlapHeight* 0.5f,               // 높이는 프로젝트에 맞게
            tilemap.cellSize.y * 0.5f * overlapShrink
        );

        // 트리거 제외가 일반적이면 Ignore, 필요하면 Collide 유지
        int hitCount = Physics.OverlapBoxNonAlloc(
            center,
            halfExtents,
            oneHit,// 1칸 버퍼
            Quaternion.identity,
            unitDetectMask,
            QueryTriggerInteraction.Ignore      // isTirgger 콜라이더는 제외(마우스 포인터의 경우 isTrigger로 제외됨)
        );

        if (hitCount <= 0) return null;

        var col = oneHit[0];
        if (!col) return null;

        // 빠른 경로: 바로 붙은 컴포넌트
        if (col.TryGetComponent<T>(out var direct))
            return direct;

        // 예외적으로 자식/부모에 있을 수 있으므로 부모에서 한 번만 검색
        return col.GetComponentInParent<T>(true);
    }

    // ====== Player Range 토글 유틸 ======
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

    // ====== Enemy Popup 토글 유틸 ======
    private void ShowEnemyPopup(BaseEnemy enemy)
    {
        if (enemy == null) return;
        selectedEnemy = enemy;
        enemyPopupVisible = true;
        GameManager.UI.GetUI<EnemyInfoPopUpUI>()
            .SetData(enemy.enemyModel.unitName, enemy.enemyModel.attri, enemy.enemyModel.rank);
        GameManager.UI.OpenUI<EnemyInfoPopUpUI>();
    }
    private void HideEnemyPopup()
    {
        enemyPopupVisible = false;
        selectedEnemy = null;
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();
    }
    
    // 공격범위안에 있다는 bool 메서드
    private bool IsCellInAttackOrKickRange(Vector3Int cell)
    {
        return map != null 
               && map.attackRangeTilemap != null 
               && map.attackRangeTilemap.HasTile(cell);
    }
    //공격 범위 끄기
    private void CancelAttackOrKickRange()
    {
        if (GameManager.Map != null && GameManager.Map.attackRange != null)
        {
            GameManager.Map.attackRange.ClearAttackType(); // 내부적으로 타일/타겟도 비움
        }
    }
    
    
}
