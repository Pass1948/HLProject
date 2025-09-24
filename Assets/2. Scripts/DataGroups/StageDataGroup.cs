using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class StageDataGroup : StageData
{
    public StageData GetStageData(int idx)
    {
        return StageDataMap[idx];
    }
}
