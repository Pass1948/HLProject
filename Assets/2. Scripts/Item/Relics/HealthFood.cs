using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFood : MulliganItem
{
    protected override void OnEnable()
    {
        base.OnEnable();    
        AddMulligan(relicItems, 3006);
    }

    private void OnDisable()
    {
        RemoveMulligan(relicItems, 3006);
    }
    private void OnDestroy()
    {
        RemoveMulligan(relicItems, 3006);
    }
}
