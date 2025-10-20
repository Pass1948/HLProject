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
        slots = new RewardSlotUI[rewardPanel.childCount];
        //슬롯 생성 차일드 카운트를 for문으로
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(true);
            
        }
    }
    
    private void OpenStore()
    {
        GameManager.UI.GetUI<ShopUI>();
        GameManager.Sound.PlayBGM(GameManager.Resource.Create<AudioClip>(Path.Sound + "Buy some cards!"));
        GameManager.Sound.PlayUISfx();
    }

}
