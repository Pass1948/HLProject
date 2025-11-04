using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AceAmplifier : BaseItem
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Add(relicItems, 3019);
    }

    private void OnDisable()
    {
        Remove(relicItems, 3019);
    }
    private void OnDestroy()
    {
        Remove(relicItems, 3019);
    }

    protected virtual void Add(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.ItemControl.ace = true;
            }
        }

    }
    protected virtual void Remove(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.ItemControl.ace = false;
            }
        }
    }
}
