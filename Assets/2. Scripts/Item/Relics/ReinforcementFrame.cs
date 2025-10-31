using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcementFrame : BaseItem
{

    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3010);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3010);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3010);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Vehicle.vehicleModel.maxHealth += items[i].addBikeHealth;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Vehicle.vehicleModel.maxHealth -= items[i].addBikeHealth;
            }
        }
    }
}
