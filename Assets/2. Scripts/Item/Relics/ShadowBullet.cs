using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBullet : BaseItem
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3017);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3017);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3017);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.ItemControl.spade = true;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.ItemControl.spade = false;
            }
        }
    }
}
