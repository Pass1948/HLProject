using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class RelicDataGroup : RelicData
{
    public RelicData GetEntityData(int idx)
    {
        return RelicDataMap[idx];
    }
}
