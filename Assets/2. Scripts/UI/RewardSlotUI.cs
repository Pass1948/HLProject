using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotUI : BaseUI
{
    [SerializeField] Button selectButton;
    [SerializeField] Image rewardIcon;
    [SerializeField] TextMeshProUGUI rewardText;

    protected override void OnOpen()
    {
        base.OnOpen();

        selectButton.onClick.AddListener(SelectReward);

        GetRewardInfo();
    }

    protected override void OnClose()
    {
        base.OnClose();

        selectButton.onClick.RemoveAllListeners();
    }

    private void GetRewardInfo()
    {
        // 리워드(유물, 재화 등) 의 정보 받아서 UI반영
        // rewardIcon =
        // rewardText.text =

    }

    private void SelectReward()
    {
        // 보상을 얻어서 적용 시키는 로직 적용
    }
}
