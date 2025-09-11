using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathPreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject pathTilePrefab;
    private readonly List<GameObject> activeTiles = new List<GameObject>();
    public void ShowPath(List<Vector3Int> path, Tilemap tilemap)
    {
        ClearPath();
        foreach (var cell in path)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
            var tileObj = GameManager.Resource.Create<GameObject>("GreenTile");
            tileObj.transform.position = worldPos;
            activeTiles.Add(tileObj);
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
