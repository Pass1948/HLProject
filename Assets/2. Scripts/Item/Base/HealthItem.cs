using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : BaseItem
{
    protected virtual void AddHealth(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                playerModel.maxHealth += items[i].addMaxHealth;
                playerModel.currentHealth += items[i].addMaxHealth;
            }
        }
        
    }
    protected virtual void RemoveHealth(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                playerModel.maxHealth -= items[i].addMaxHealth;
            }
        }
    }
}
