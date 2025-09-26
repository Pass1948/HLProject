using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class BaseObstacle : MonoBehaviour
{
    public Vector3Int gridPos;
    public ObstacleModel obstacleModel; // 💡 모델 컨테이너 역할
    
    public void InitObstacle(Vector3Int pos, ObstacleData data)
    {
        
        obstacleModel = new ObstacleModel();
        obstacleModel.InitData(data); 

        // 위치 설정
        gridPos = pos;
        SetPosition(pos);
    }
    
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
