using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedEngine : BaseItem
{

    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3011);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3011);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3011);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Vehicle.vehicleModel.moveRange += items[i].addMoveRange;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Vehicle.vehicleModel.moveRange -= items[i].addMoveRange;
            }
        }
    }
}
