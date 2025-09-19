using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearUI : BaseUI
{
    [SerializeField] Button rewardPassButton;
    [SerializeField] Transform rewardPanel;
    [SerializeField] RewardSlotUI[] slots;

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

    }

}
