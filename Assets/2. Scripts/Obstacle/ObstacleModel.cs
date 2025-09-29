using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class ObstacleModel : UnitModel 
{
    // 종류
    public ObstacleType obstacleType; 
    
    // 이동 가능한지
    public bool isPlaceable = false;      
    
    // 파괴 가능한지
    public bool isDestructible = false;   

    
    public void InitData(ObstacleData data)
    {
        
        id = data.id;
        unitType = UnitType.Obstacle;
        size = data.size;
        unitName = data.name;
        
        isDestructible = data.isDestructible == 1;
        obstacleType = data.type;
        isPlaceable = data.canPlaceUnit == 1;
    }
}