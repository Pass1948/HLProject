using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShoes : BaseItem
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Update()
    {
        AddFirst(relicItems, 3013);
    }

    protected virtual void AddFirst(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                if(GameManager.TurnBased.turnCount <= 1)
                {
                    GameManager.Unit.Player.playerModel.moveRange += items[i].addMoveRange;
                }
                else
                {
                    GameManager.Unit.Player.playerModel.moveRange -= items[i].addMoveRange;
                }
            }
        }

    }
}
