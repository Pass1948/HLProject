using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horn_RimmedGlasses : RangeItem
{
    protected override void OnEnable()
    {
        base.OnEnable();    
        AddAttackRange(relicItems, 3004);
    }

    private void OnDisable()
    {
        RemoveAttackRange(relicItems, 3004);
    }
    private void OnDestroy()
    {
        RemoveAttackRange(relicItems, 3004);
    }
}
