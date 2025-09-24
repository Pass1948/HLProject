using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTable;
public class BulletDataGroup : BulletData
{
    public BulletData GetBulletData(int idx)
    {
        return BulletDataMap[idx];
    }
}
