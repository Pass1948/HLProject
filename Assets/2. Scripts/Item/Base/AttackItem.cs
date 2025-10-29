using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackItem : BaseItem
{
    protected virtual void AddAttack(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Player.playerModel.attack +=items[i].addAttack;
            }
        }
        
    }
    protected virtual void RemoveAttack(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Player.playerModel.attack -=items[i].addAttack;
            }
        }
    }
}
