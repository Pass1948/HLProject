using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullPoket : MulliganItem
{
    protected override void OnEnable()
    {
        base.OnEnable();    
        AddMulligan(relicItems, 3003);
    }

    private void OnDisable()
    {
        RemoveMulligan(relicItems, 3003);
    }
    private void OnDestroy()
    {
        RemoveMulligan(relicItems, 3006);
    }
}
