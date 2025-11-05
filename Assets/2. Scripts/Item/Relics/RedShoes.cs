using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShoes : BaseItem
{
    bool isMove = false;

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
                if(GameManager.TurnBased.turnCount <= 1 && !isMove)
                {
                    isMove = true;
                    GameManager.Unit.Player.playerModel.moveRange += items[i].addMoveRange;
                }
                else
                {
                    isMove = false;
                    GameManager.Unit.Player.playerModel.moveRange -= items[i].addMoveRange;
                }
            }
        }

    }
}
