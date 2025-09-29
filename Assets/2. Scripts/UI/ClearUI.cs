using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearUI : BaseUI
{
    [SerializeField] Button storeButton;
    [SerializeField] Transform rewardPanel;
    [SerializeField] RewardSlotUI[] slots;

    protected override void OnOpen()
    {
        base.OnOpen();
        storeButton.onClick.AddListener(OpenStore);
    }

    protected override void OnClose()
    {
        base.OnClose();
    }

    private void CreateSlots()
    {

    }
    
    private void OpenStore()
    {
        GameManager.UI.OpenUI<ShopUI>();
    }

}
