using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverUI : BaseUI
{
    [SerializeField] Button mainmenuButton;

    private TestScene testScene;
    protected override void OnOpen()
    {
        base.OnOpen();
        mainmenuButton.onClick.AddListener(MainmenuScene);
    }

    protected override void OnClose()
    {
        base.OnClose();
        mainmenuButton.onClick.RemoveAllListeners();
    }

    private void MainmenuScene()
    {
        // 메인메뉴 (인트로?) 씬으로 
        Debug.Log("메인메뉴로~");
        GameManager.SceneLoad.LoadScene(SceneType.Title);
    }

}
