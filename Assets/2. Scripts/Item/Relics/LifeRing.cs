using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeRing : HealthItem
{
    protected override void OnEnable()
    {
        base.OnEnable();
        AddHealth(relicItems, 3005);
    }

    private void OnDisable()
    {
        RemoveHealth(relicItems, 3005);
    }
    private void OnDestroy()
    {
        RemoveHealth(relicItems, 3005);
    }
}
