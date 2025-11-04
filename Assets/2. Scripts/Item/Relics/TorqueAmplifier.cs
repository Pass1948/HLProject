using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueAmplifier : BaseItem
{
    bool isAdd = true;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Update()
    {
        Add(relicItems, 3012);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                if(GameManager.Unit.Vehicle.vehicleModel.condition == VehicleCondition.GetOff&& isAdd)
                {
                    isAdd = false;
                    GameManager.Unit.Player.playerModel.moveRange += items[i].addMoveRange;
                }

                if (GameManager.Unit.Vehicle.vehicleModel.condition == VehicleCondition.Riding&& !isAdd)
                {
                    isAdd = true;
                    GameManager.Unit.Player.playerModel.moveRange -= items[i].addMoveRange;
                }
            }
        }

    }
}
