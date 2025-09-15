using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathPreviewManager : MonoBehaviour
{
    private readonly List<GameObject> activeTiles = new List<GameObject>();

    public void ShowPath(List<Vector3Int> path, Tilemap tilemap, int moveRange)
    {
        ClearPath();

        // 이동 범위 제한
        int maxRange = Mathf.Min(path.Count, moveRange);
        for (int i = 0; i < path.Count; i++)
        {
            if (tilemap == null) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(path[i]);
            if (i < moveRange)
            {
                var tileObj = GameManager.Resource.Create<GameObject>(Path.Map + "GreenTile");
                tileObj.transform.position = worldPos;
                activeTiles.Add(tileObj);
            }
            else
            {
                var tileObj = GameManager.Resource.Create<GameObject>(Path.Map +"RedTile");
                tileObj.transform.position = worldPos;
                activeTiles.Add(tileObj);
            }
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
