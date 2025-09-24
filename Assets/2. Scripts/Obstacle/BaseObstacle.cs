using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObstacle : MonoBehaviour
{
    public Vector3Int gridPos;
    
    public virtual void Init(Vector3Int pos)
    {
        gridPos = pos;
        SetPosition(pos);
    }

    // 위치
    public void SetPosition(Vector3Int pos)
    {
        gridPos = pos;
        transform.position = GameManager.Map.tilemap.GetCellCenterWorld(gridPos);
    }
    
}
