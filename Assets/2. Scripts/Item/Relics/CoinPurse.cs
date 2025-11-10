using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPurse : BaseItem
{
    protected override void OnEnable()
    {
        base.OnEnable();
        AddCoin(relicItems, 3007);
    }

    private void OnDisable()
    {
        RemoveCoin(relicItems, 3007);
    }
    private void OnDestroy()
    {
        RemoveCoin(relicItems, 3007);
    }

    protected virtual void AddCoin(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.TurnBased.turnSettingValue.rewardGold += items[i].moneyBonus;
            }
        }

    }
    protected virtual void RemoveCoin(List<ItemModel> items, int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.TurnBased.turnSettingValue.rewardGold -= items[i].moneyBonus;
            }
        }
    }
}
