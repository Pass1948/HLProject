using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotUI : BaseUI
{
    [SerializeField] Button selectButton;
    [SerializeField] Image rewardIcon;
    [SerializeField] TextMeshProUGUI rewardText;
    
    private object rewardItem;

    private void Start()
    {
    }
    public void SetReward(object reward)
    {
        gameObject.SetActive(true);
        rewardItem = reward;
        if(reward is Ammo ammo)
        {
            rewardText.text = $"{ammo.suit}{ammo.rank}";
        }
        else if (reward is ItemModel relic)
        {
            rewardText.text = relic.name;
            
            // TODO: 추가 해야함. 사진(JBS)
            rewardIcon.sprite = GameManager.Resource.Load<Sprite>(Path.UISprites);
        }
    }
    protected override void OnOpen()
    {
        base.OnOpen();
        selectButton.onClick.AddListener(SelectReward);
    }
    
    protected override void OnClose()
    {
        base.OnClose();
        selectButton.onClick.RemoveAllListeners();
    }
    private void SelectReward()
    {
        if (rewardItem is Ammo ammo)
        {
            GameManager.ItemControl.drawPile.Add(ammo);
            Debug.Log(ammo.suit);
        }
        else if (rewardItem is ItemModel relic)
        {
            GameManager.ItemControl.buyItems.Add(relic);
            GameManager.ItemControl.CreateRelicObject(relic.id, relic);
        }
        gameObject.SetActive(false);
    }
    
    
}
