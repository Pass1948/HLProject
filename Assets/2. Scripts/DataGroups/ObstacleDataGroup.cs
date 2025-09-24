using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class ObstacleDataGroup : ObstacleData
{
    public ObstacleData GetObstacleData(int idx)
    {
        return ObstacleDataMap[idx];
    }
}
