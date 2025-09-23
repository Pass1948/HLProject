using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;

public class EntityDataGroup : EntityData
{
    public EntityData GetEntityData(int idx)
    {
        return EntityDataMap[idx];
    }
}
