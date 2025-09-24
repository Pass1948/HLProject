using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class RelicDataGroup : RelicData
{
    public RelicData GetRelicData(int idx)
    {
        return RelicDataMap[idx];
    }
}
