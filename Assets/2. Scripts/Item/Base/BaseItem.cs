using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    public ItemModel itemModel;
    public List<ItemModel> relicItems = GameManager.ItemControl.relicItems;
    public PlayerModel playerModel = GameManager.Unit.Player.playerModel;
    public EntityData entityData;
    public int id;
    protected virtual void OnEnable()
    {
        // 초기 플레이어 스텟 되돌리기 때문에 정의함
        entityData = GameManager.Data.entityDataGroup.GetEntityData(1001);  // 플레이어 데이터 바인딩
    }
}
