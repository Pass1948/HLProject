using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrugalityDie : BaseItem
{

    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3009);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3009);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3009);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Shop.rerollCost -= items[i].moneyBonus;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Shop.rerollCost += items[i].moneyBonus;
            }
        }
    }
}
