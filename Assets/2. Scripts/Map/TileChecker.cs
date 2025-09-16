using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChecker : MonoBehaviour
{
    
    public Camera mainCamera; // 카메라
    
    void Update()
    {
        Tilemap currentTilemap = GameManager.Map.tilemap;
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitWorldPos = hit.point;
                
                Vector3Int cellPos = currentTilemap.WorldToCell(hitWorldPos);
                
                int tileID = GameManager.Map.mapData[cellPos.x, cellPos.y];
                
                Debug.Log($"좌표: ({cellPos.x}, {cellPos.y}, TileID: {tileID})");
            }
        }
    }
}