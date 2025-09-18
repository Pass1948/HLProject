using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public static class MouseToWorldPosition 
{
    static Camera cam;
    public static void Init(Camera c = null) => cam = c ? c : Camera.main;

    public static bool TryRaycastWorld(out Vector3 world, LayerMask mask, float maxDist = 1000f, bool uiBlocks = true)
    {
        if (uiBlocks && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        { world = default; return false; }

        Vector2 mousePos =
#if ENABLE_INPUT_SYSTEM
            Mouse.current.position.ReadValue();
#else
            (Vector2)Input.mousePosition;
#endif
        Ray ray = (cam ? cam : Camera.main).ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out var hit, maxDist, mask, QueryTriggerInteraction.Ignore))
        { world = hit.point; return true; }

        world = default; return false;
    }

    public static bool TryPlaneWorld(out Vector3 world, float y = 0f, bool uiBlocks = true)
    {
        if (uiBlocks && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        { world = default; return false; }

        Vector2 mousePos =
#if ENABLE_INPUT_SYSTEM
            Mouse.current.position.ReadValue();
#else
            (Vector2)Input.mousePosition;
#endif
        Ray ray = (cam ? cam : Camera.main).ScreenPointToRay(mousePos);
        var plane = new Plane(Vector3.up, new Vector3(0f, y, 0f));
        if (plane.Raycast(ray, out float enter))
        { world = ray.GetPoint(enter); return true; }

        world = default; return false;
    }

    public static Vector3 SnapToGrid(Vector3 w, float cellX, float cellZ, float y)
    {
        w.y = y;
        w.x = Mathf.Round(w.x / cellX) * cellX;
        w.z = Mathf.Round(w.z / cellZ) * cellZ;
        return w;
    }

    public static Vector3 SnapToTilemap(Tilemap tm, Vector3 w)
    {
        var c = tm.WorldToCell(w);
        return tm.GetCellCenterWorld(c);
    }
}
