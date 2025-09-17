using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MouseFollower : MonoBehaviour
{
    [Header("Target")]
    public Transform followTarget;       // 비워두면 이 오브젝트가 이동
    public Camera cam;

    [Header("Raycast (Collider 필요)")]
    public bool useLayerRaycast = true;
    public LayerMask groundMask = ~0;    // Ground 등만 체크
    public float maxRayDistance = 500f;

    [Header("Plane Fallback (Collider 없어도 동작)")]
    public bool usePlaneIfMiss = true;
    public float groundY = 0f;

    [Header("Grid / Tilemap Snap")]
    public bool snapToGrid = false;
    public Vector2 cellSize = new Vector2(1, 1);
    public Tilemap tilemap;              // isometric이면 Grid Cell Swizzle XZY 권장

    [Header("NavMesh Clamp (도달가능 구간만)")]
    public bool clampToNavMesh = false;
    public float navMeshMaxDistance = 2f;

    [Header("Smoothing")]
    public bool smooth = true;
    public float smoothTime = 0.05f;
    public float maxSpeed = 100f;
    Vector3 vel;

    void Awake()
    {
        if (!followTarget) followTarget = transform;
        MouseWorld.Init(cam);
    }

    void LateUpdate()
    {
        if (!TryGetMouseWorld(out var world)) return;

        // 스냅 옵션
        if (tilemap) world = MouseWorld.SnapToTilemap(tilemap, world);
        else if (snapToGrid) world = MouseWorld.SnapToGrid(world, cellSize.x, cellSize.y, groundY);
        else world.y = groundY; // 필요 시 고정

        // NavMesh 안으로 보정
        if (clampToNavMesh && NavMesh.SamplePosition(world, out var hit, navMeshMaxDistance, NavMesh.AllAreas))
            world = hit.position;

        // 이동
        if (!smooth) followTarget.position = world;
        else followTarget.position = Vector3.SmoothDamp(followTarget.position, world, ref vel, smoothTime, maxSpeed, Time.deltaTime);
    }

    bool TryGetMouseWorld(out Vector3 world)
    {
        if (useLayerRaycast && MouseWorld.TryRaycastWorld(out world, groundMask, maxRayDistance, true))
            return true;
        if (usePlaneIfMiss && MouseWorld.TryPlaneWorld(out world, groundY, true))
            return true;

        world = default;
        return false;
    }
}
