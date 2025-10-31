using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueAmplifier : BaseItem
{

    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3012);
        Remove(relicItems, 3012);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3012);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3012);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                if(GameManager.Unit.Vehicle.vehicleModel.condition ==VehicleCondition.GetOff)
                GameManager.Unit.Player.playerModel.moveRange += items[i].addMoveRange;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                if (GameManager.Unit.Vehicle.vehicleModel.condition == VehicleCondition.Riding)
                    GameManager.Unit.Player.playerModel.moveRange -= items[i].addMoveRange;
            }
        }
    }
}
