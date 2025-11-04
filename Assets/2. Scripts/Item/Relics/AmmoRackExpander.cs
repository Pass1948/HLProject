using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRackExpander : BaseItem
{

    protected override void OnEnable()
    {
        base.OnEnable();
        AddCoin(relicItems, 3008);
    }

    private void OnDisable()
    {
        RemoveCoin(relicItems, 3008);
    }
    private void OnDestroy()
    {
        RemoveCoin(relicItems, 3008);
    }

    protected virtual void AddCoin(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Shop.bulletCount += items[i].addMaxBullet;
            }
        }

    }
    protected virtual void RemoveCoin(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Shop.bulletCount -= items[i].addMaxBullet;
            }
        }
    }
}
