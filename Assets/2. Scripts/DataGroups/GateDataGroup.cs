using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class GateDataGroup : GateData
{
    public GateData GetGateData(int idx)
    {
        return GateDataMap[idx];
    }
    
}
