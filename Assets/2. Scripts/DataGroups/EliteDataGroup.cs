using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class EliteDataGroup : EliteData
{
    public EliteData GetEliteData(int idx)
    {
        return EliteDataMap[idx];
    }
    

}
