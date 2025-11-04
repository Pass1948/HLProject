using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeItem : BaseItem
{
    
    // =====================================================================
    // 이동범위
    // =====================================================================
    protected virtual void AddMoveRange(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Player.playerModel.moveRange += items[i].addMoveRange;

            }
        }
        
    }
    protected virtual void RemoveMoveRange(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Player.playerModel.moveRange -=items[i].addMoveRange;
            }
        }
    }
    
    // =====================================================================
    // 공격범위
    // =====================================================================
    protected virtual void AddAttackRange(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Player.playerModel.attackRange +=items[i].addAttackRange;
                Debug.Log("내 사거리 싯팔" + items[i].addAttackRange + GameManager.Unit.Player.playerModel.attackRange);
            }
        }
        
    }
    protected virtual void RemoveAttackRange(List<ItemModel> items, int id)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                GameManager.Unit.Player.playerModel.attackRange -=items[i].addAttackRange;
            }
        }
    }

}
