using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverUI : BaseUI
{
    [SerializeField] Button mainmenuButton;

    private TestScene testScene;

    private void Awake()
    {
        mainmenuButton.onClick.AddListener(MainmenuScene);
    }
    protected override void OnOpen()
    {
        base.OnOpen();
    }

    protected override void OnClose()
    {
        base.OnClose();
        mainmenuButton.onClick.RemoveAllListeners();
    }

    private void MainmenuScene()
    {
        // 메인메뉴 (인트로?) 씬으로 
        GameManager.ItemControl.ClearData();
        GameManager.Unit.isRiding = false;
        GameManager.TurnBased.turnSettingValue.isDeck = false;
        GameManager.ItemControl.ClearData();
        GameManager.SceneLoad.LoadScene(SceneType.Title);
    }

}
