using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HolySaw : AttackItem
{
    protected override void OnEnable()
    {
        base.OnEnable();    
        AddAttack(relicItems, 3001);   
    }

    private void OnDisable()
    {
        RemoveAttack(relicItems, 3001);
    }
}
