using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class ClearUI : BaseUI
{
    [SerializeField] Button storeButton;
    [SerializeField] Transform rewardPanel;
    [SerializeField] RewardSlotUI[] slots;

    private void OnEnable()
    {
        storeButton.onClick.AddListener(OpenStore);
        OnAnalyticsEvent(GameManager.SaveLoad.nextSceneIndex);
    }
    private void OnAnalyticsEvent(int v)
    {
        //TODO: stage_clear_popup
        CustomEvent customEvent = new CustomEvent("stage_clear_popup")
        {
            { "stageValue", v}
        };
        AnalyticsService.Instance.RecordEvent(customEvent);

    }
    private void OnDisable()
    {
        storeButton.onClick.RemoveListener(OpenStore);
    }


    protected override void OnOpen()
    {
        base.OnOpen();
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
        OnAnalyticsEvent_next(GameManager.SaveLoad.nextSceneIndex);
        OnAddPile();
        GameManager.UI.GetUI<ShopUI>();
        GameManager.Sound.PlayUISfx();
    }
    private void OnAnalyticsEvent_next(int v)    
    {
        //TODO: stage_next_click
        CustomEvent customEvent = new CustomEvent("stage_next_click")
        {
            { "stageValue", v}
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
    }   

    private void OnAddPile()
    {
        for(int i = 0; i < GameManager.ItemControl.discardPile.Count; i++)
        {
            GameManager.ItemControl.drawPile.Add(GameManager.ItemControl.discardPile[i]);
        }
        GameManager.ItemControl.discardPile.Clear();
    }
}
