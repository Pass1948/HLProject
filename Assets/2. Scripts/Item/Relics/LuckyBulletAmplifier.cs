using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyBulletAmplifier : BaseItem
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3015);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3015);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3015);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.ItemControl.club = true;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.ItemControl.club = false;
            }
        }
    }
}
