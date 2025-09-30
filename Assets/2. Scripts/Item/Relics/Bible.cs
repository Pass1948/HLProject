using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bible : HealthItem
{
    protected override void OnEnable()
    {
        base.OnEnable();   
        AddHealth(relicItems, 3006);
    }

    private void OnDisable()
    {
        RemoveHealth(relicItems, 3006);
    }
}
