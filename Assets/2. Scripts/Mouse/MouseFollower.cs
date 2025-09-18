using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseFollower : MonoBehaviour
{
    [Header("References")]
    public MapManager map;          //  GameManager.Map 
    public Tilemap tilemap;         // map.tilemap 
    public Transform pointer;       //  this.transform
    public Camera cam;              //  Camera.main
    public MovementController movement; 

    [Header("Raycast / Plane")]
    public bool useLayerRaycast = true;
    public LayerMask groundMask = ~0;     // 필요 시 Ground, TileMap만 포함
    public float maxRayDistance = 1000f;
    public bool usePlaneIfMiss = true;
    public float groundY = 0f;

    [Header("Constraints")]
    public bool blockWhenUI = true;       // UI 위에서 멈춤
    public bool clampToBounds = true;     // 경계 밖이면 모서리 칸으로 클램프
    public bool freezeWhenOutside = false;// 경계 밖이면 아예 정지(마지막 유효칸 유지)
    public bool restrictToMovable = false;// Terrain(이동가능) 셀만 허용

    [Header("Visual")]
    public bool tintBlocked = true;       // 불가 칸이면 색상 변경
    public Color movableColor = Color.white;
    public Color blockedColor = new Color(1, 0.5f, 0.5f);

    [Header("Instance Lookup (보강용)")]
    public LayerMask unitDetectMask = ~0; // Enemy/Player 찾을 물리마스크
    [Range(0.1f, 1f)] public float overlapShrink = 0.9f; // 셀 박스 축소율
    public float overlapHeight = 2f;      // OverlapBox Y크기

    // 내부 상태
    private Vector3Int _lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private Vector3Int _lastValidCell;
    private Renderer _pointerRenderer;

    void Awake()
    {
        if (!pointer) pointer = transform;
        if (!map) map = GameManager.Map;
        if (!tilemap) tilemap = map ? map.tilemap : null;
        if (!cam) cam = Camera.main;
        if (!movement) movement = FindObjectOfType<MovementController>();
        _pointerRenderer = pointer.GetComponentInChildren<Renderer>();
        if (!map || !tilemap || !movement)
            Debug.LogWarning("[MouseFollower] 참조 누락(map/tilemap/movement).");
    }

    void Update()
    {
        if (blockWhenUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        if (!TryGetMouseWorld(out var world)) return;

        // 월드 → 셀
        Vector3Int cell = tilemap.WorldToCell(world);

        // 경계 판단
        bool inside = IsInside(cell);
        if (!inside)
        {
            if (freezeWhenOutside)
            {
                // 경계 밖이면 위치 갱신 안 함(마지막 유효칸 유지)
                ApplyTint(_lastValidCell, allowed: true);
                return;
            }
            if (clampToBounds)
            {
                cell = ClampCell(cell);
                inside = true;
            }
        }

        // 이동 가능 셀 제한(옵션)
        bool allowed = true;
        if (restrictToMovable && inside)
            allowed = map.IsMovable(cell);

        // 셀 변경시에만 “한 칸 이동”
        if (allowed && cell != _lastCell)
        {
            GridSnapper.SnapToCellCenter(pointer, tilemap, new Vector2Int(cell.x, cell.y));
            _lastCell = cell;
            _lastValidCell = cell;
        }

        // 비주얼(불가 칸일 때 색)
        ApplyTint(cell, allowed);
    }

    // === Helpers ===
    bool TryGetMouseWorld(out Vector3 world)
    {
        world = default;

        Vector2 mousePos =
#if ENABLE_INPUT_SYSTEM
            Mouse.current.position.ReadValue();
#else
            (Vector2)Input.mousePosition;
#endif
        Ray ray = cam.ScreenPointToRay(mousePos);

        // 1) 레이어 레이캐스트
        if (useLayerRaycast && Physics.Raycast(ray, out var hit, maxRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            world = hit.point; return true;
        }

        // 2) 평면 교차 (콜라이더가 없어도 동작)
        if (usePlaneIfMiss)
        {
            var plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
            if (plane.Raycast(ray, out float enter))
            { world = ray.GetPoint(enter); return true; }
        }

        return false;
    }

    bool IsInside(Vector3Int c)
    {
        return c.x >= 0 && c.y >= 0 && c.x < map.mapWidth && c.y < map.mapHeight;
    }

    Vector3Int ClampCell(Vector3Int c)
    {
        int x = Mathf.Clamp(c.x, 0, map.mapWidth - 1);
        int y = Mathf.Clamp(c.y, 0, map.mapHeight - 1);
        return new Vector3Int(x, y, 0);
    }

    void ApplyTint(Vector3Int cell, bool allowed)
    {
        if (!tintBlocked || _pointerRenderer == null) return;
        _pointerRenderer.sharedMaterial.color = allowed ? movableColor : blockedColor;
    }

    /// <summary>현재 포인터가 위치한(혹은 마지막 유효) 셀을 외부에서 얻고 싶을 때</summary>
    public Vector3Int GetCurrentCell(bool preferValid = true)
    {
        return preferValid ? (_lastValidCell == default ? _lastCell : _lastValidCell) : _lastCell;
    }
}
