using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    //[HideInInspector] 테스터 이후 가림
    public GameObject mouse;
    // 타일맵 위에서 타일과 오브젝트 탐색하는 탐색기 역할 매니저
    // 오브젝트가 마우스에 따라다니도록 하는 기능 구현 필요


    private readonly List<GameObject> activeTiles = new List<GameObject>();

    private void OnEnable()
    {
        mouse = GameManager.Resource.Create<GameObject>(Path.Mouse+"Pointer");
        transform.SetParent(mouse.transform);
    }


    private void LateUpdate()
    {
        
    }

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
