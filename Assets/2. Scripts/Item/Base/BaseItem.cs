using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public ItemModel itemModel;

    private void Awake()
    {
        GameManager.ItemControl.BaseItem = this;
    }

    public void InitItem(RelicData data)    // 이곳에서 호출하는 이유는 오브젝트들을 개별적으로 리스트에 담기 위함임
    {
        if (!GameManager.ItemControl.items.Contains(this))
            GameManager.ItemControl.items.Add(this);

        itemModel = new ItemModel();
        itemModel.InitData(data);
    }
}
