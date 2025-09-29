using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulliganItem : BaseItem
{
    protected virtual void AddMulligan(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                playerModel.reload +=items[i].addMulligan;
            }
        }
        
    }
    protected virtual void RemoveMulligan(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                playerModel.reload -=items[i].addMulligan;
            }
        }
    }

}
