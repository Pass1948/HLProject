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
    [Header("References")]
    [SerializeField] public MapManager map;
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public Camera cam;
    [SerializeField] public Transform pointer;
    public Vector3Int PointerCell => tilemap.WorldToCell(pointer.position);

    [Header("Raycast / Plane")]
    public bool useLayerRaycast = true;
    public LayerMask groundMask = ~0;
    public float maxRayDistance = 1000f;
    public bool usePlaneIfMiss = true;
    public float groundY = 0f;

    [Header("Constraints")]
    public bool blockWhenUI = true;
    public bool clampToBounds = true;
    public bool freezeWhenOutside = false;
    public bool restrictPointerToMovable = false;

    [Header("Instance Lookup (Optional)")]
    public bool useOverlapLookup = true;
    public LayerMask unitDetectMask = ~0;
    [Range(0.1f, 1f)] public float overlapShrink = 0.9f;
    public float overlapHeight = 2f;

    [Header("Movement")]
    public float stepMoveTime = 0.2f;
    private bool isMoving = false;
    public bool movePhaseActive = false;   // 이 값이 false면 이동 안 됨
    private BasePlayer selectedPlayer;
    private Vector3Int selectedPlayerCell;
    private int selectedMoveRange;
    private bool isPlayer = false;         // 마지막으로 클릭한게 플레이어였는지

    // 공격/킥 플래그 (기존 유지)
    private bool isAttacking = false;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    private bool isKicking = false;
    public bool IsKicking { get => isKicking; set => isKicking = value; }

    // 내부 캐시
    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Vector3Int lastValidCell;

    // UI/토글
    private bool playerRangeVisible = false;
    private BaseEnemy selectedEnemy;
    private bool enemyPopupVisible = false;
    public bool isMouse = false;
    public bool isShowRange = true;  // ← 이동범위 보일 의도면 true로 두세요 (기본 false면 안 보입니다)

    // hover
    private Vector3Int _hoverCell;
    private bool _hasHover;

    private readonly Collider[] oneHit = new Collider[8];

    public void CreateMouse()
    {
        var go = GameManager.Resource.Create<GameObject>(Path.Mouse + "Pointer");
        go.transform.SetParent(this.transform);
        pointer = go.transform;
    }

    public void SetMouseVar()
    {
        map = GameManager.Map;
        tilemap = map.tilemap;
        cam = Camera.main;
    }

    // ========== InputSystem → MouseFollower에서 호출 ==========
    public void UpdatePointerFromScreen(Vector2 screen)
    {
        if (pointer == null || tilemap == null || map == null || cam == null) return;

        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        { _hasHover = false; return; }

        if (!TryGetMouseWorld(screen, out var world))
        { _hasHover = false; return; }

        var cell = tilemap.WorldToCell(world);

        bool inside = IsInside(cell);
        if (!inside)
        {
            if (freezeWhenOutside) { _hasHover = false; return; }
            if (clampToBounds) { cell = ClampCell(cell); inside = true; }
        }

        bool allowed = true;
        if (restrictPointerToMovable && inside)
            allowed = map.IsMovable(cell);

        if (allowed && cell != lastCell)
        {
            Vector3 center = tilemap.GetCellCenterWorld(cell);
            center.y = groundY + 0.01f;
            pointer.position = center;

            lastCell = cell;
            lastValidCell = cell;
        }

        _hoverCell = cell;
        _hasHover  = allowed && inside;
    }

    public void ClickCurrentHover()
    {
        if (!_hasHover) return;
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        HandleLeftClick(_hoverCell);
    }

    // ========== 클릭 처리 ==========
    private void HandleLeftClick(Vector3Int cell)
    {
        if (!IsInside(cell)) { CancelSelection(); return; }

        bool cellIsPlayer  = map.IsPlayer(cell);
        bool cellIsEnemy   = map.IsEnemy(cell);
        bool cellIsTerrain = map.IsMovable(cell);

        // 공격
        if (isAttacking)
        {
            if (IsCellInAttackOrKickRange(cell))
            {
                GameManager.Event.Publish(EventType.PlayerAttack);
            }
            else
            {
                CancelAttackOrKickRange();
            }
            isAttacking = false;
            return;
        }

        // 킥
        if (isKicking)
        {
            if (IsCellInAttackOrKickRange(cell))
                GameManager.TurnBased.ChangeTo<PlayerKickState>();
            else
                CancelAttackOrKickRange();

            isKicking = false;
            return;
        }

        // 일반
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

        isPlayer = false;
        CancelSelection();
    }

    // ===== 동작 핸들러 =====
    private void OnClickPlayer(Vector3Int cell)
    {
        // 공격/킥 모드면 범위만 끄고 취소
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

        selectedPlayer     = useOverlapLookup ? FindAtCell<BasePlayer>(cell) : null;
        selectedPlayerCell = cell;
        selectedMoveRange  = GameManager.Unit.Player.playerModel.moveRange;

        // 디버그: 선택 확인
        Debug.Log($"[Mouse] Player Selected? {(selectedPlayer!=null)} / cell {cell}");
        if (selectedPlayer == null)
        {
            Debug.LogWarning("[Mouse] 선택한 칸에서 플레이어 콜라이더를 찾지 못했습니다. unitDetectMask / OverlapBox 범위를 확인하세요.");
        }

        if (!isShowRange) return;

        if (movePhaseActive)
        {
            if (playerRangeVisible && selectedPlayerCell == cell)
                HidePlayerRange();
            else
                ShowPlayerRange(cell);
        }
    }

    private void OnClickEnemy(Vector3Int cell)
    {
        HidePlayerRange();
        if (isMoving) return;

        var enemy = useOverlapLookup ? FindAtCell<BaseEnemy>(cell) : null;
        if (enemy == null) { HideEnemyPopup(); CancelSelection(); return; }

        if (enemyPopupVisible && selectedEnemy == enemy) HideEnemyPopup();
        else ShowEnemyPopup(enemy);
    }

    private void OnClickTerrain(Vector3Int destCell)
    {
        GameManager.UI.CloseUI<EnemyInfoPopUpUI>();

        // 이동이 안 되는 대부분의 원인: 아래 가드 2개
        if (!isPlayer)            { Debug.Log("[Mouse] 이동 불가: 마지막 클릭 대상이 Player가 아님"); return; }
        if (!movePhaseActive)     { Debug.Log("[Mouse] 이동 불가: movePhaseActive=false (PlayerMove 페이즈 아님)"); return; }
        if (selectedPlayer == null){ Debug.Log("[Mouse] 이동 불가: selectedPlayer=null"); return; }
        if (destCell == selectedPlayerCell) return;

        var path = map.FindPath(selectedPlayerCell, destCell);
        if (path == null || path.Count > selectedMoveRange)
        {
            Debug.Log("[Mouse] 이동 불가: 경로가 없거나 이동 범위를 초과");
            CancelSelection();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(MoveAlongPath(selectedPlayer.transform, selectedPlayerCell, path, TileID.Player));
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Move);
        CancelSelection();
    }

    public IEnumerator MoveAlongPath(Transform actor, Vector3Int currentCell, List<Vector3Int> path, int tileIdForActor)
    {
        isMoving = true;

        foreach (var nextCell in path)
        {
            Vector3 start = actor.position;
            Vector3 end   = tilemap.GetCellCenterWorld(nextCell);

            float t = 0f;
            float dur = Mathf.Max(0.0001f, stepMoveTime);
            while (t < 1f)
            {
                t += Time.deltaTime / dur;
                actor.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
            actor.position = end;

            map.UpdateObjectPosition(currentCell, currentCell, nextCell, nextCell, tileIdForActor);

            currentCell = nextCell;
            selectedPlayerCell = nextCell;
        }
        map.ClearPlayerRange();
        isMoving = false;
    }

    // ===== 유틸 =====
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
            center, halfExtents, oneHit, Quaternion.identity, unitDetectMask, QueryTriggerInteraction.Ignore);

        if (hitCount <= 0) return null;
        Collider col = null;
        foreach (var v in oneHit)
        {
            if (v.GetComponent<BasePlayer>() == true)
            {
                col = v;
            }
        }
         Debug.Log($"이것은 수류탄이요?{col}");    
        if (!col) return null;

        if (col.TryGetComponent<T>(out var direct)) return direct;
        return col.GetComponentInParent<T>(true);
    }

    private void ShowPlayerRange(Vector3Int cell)
    {
        playerRangeVisible = true;
        map.ClearPlayerRange();
        map.PlayerUpdateRange(cell, selectedMoveRange);
    }
    public void HidePlayerRange()
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
            .SetData(enemy.enemyModel.attri, enemy.enemyModel.rank, enemy.enemyModel.attack,
                     enemy.enemyModel.moveRange, enemy.enemyModel.currentHealth, enemy.enemyModel.maxHealth);
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
        return map != null && map.attackRangeTilemap != null && map.attackRangeTilemap.HasTile(cell);
    }
    private void CancelAttackOrKickRange()
    {
        if (GameManager.Map != null && GameManager.Map.attackRange != null)
            GameManager.Map.attackRange.ClearAttackType();
    }
}
