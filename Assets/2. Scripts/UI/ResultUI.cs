using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ResultType
{
    Clear,
    Over,
    Tutorial,
    GameClear,
}

public class ResultUI : BaseUI
{
    [SerializeField] ClearUI clearUI;
    [SerializeField] OverUI overUI;
    [SerializeField] GameObject gameClearUI;
    [SerializeField] Button gameClearBtn;

    //========[튜토리얼]========
    [SerializeField] GameObject tutorialUI1;
    [SerializeField] Button tutorialBtn1;
    [SerializeField] GameObject tutorialUI2;
    [SerializeField] Button tutorialBtn2;


    public ResultType resulttype;

    private void Awake()
    {
        tutorialBtn1.onClick.AddListener(NextStage);
        tutorialBtn2.onClick.AddListener(MainmenuScene);
        gameClearBtn.onClick.AddListener(MainmenuScene);
    }

    public void GetResultType(ResultType result)
    {
        if (result == ResultType.Clear)
        {
            overUI.CloseUI();
            clearUI.OpenUI();
        }
        else if (result == ResultType.Over)
        {
            clearUI.CloseUI();
            overUI.OpenUI();
        }
        else if (result == ResultType.Tutorial)
        {
            if(GameManager.Shop.isTutorial1 == true)
            {
                Debug.Log("튜토리얼 1");
                tutorialUI1.SetActive(true);
                clearUI.CloseUI();
                overUI.CloseUI();
            }
            if(GameManager.Shop.isTutorial1 == false)
            {
                tutorialUI1.SetActive(false);
                tutorialUI2.SetActive(true);
                clearUI.CloseUI();
                overUI.CloseUI();
            }
        }
        else if (result == ResultType.GameClear)
        {
            if (GameManager.TurnBased.turnSettingValue.isTutorial == true) return;
            gameClearUI.SetActive(true);
            tutorialUI1.SetActive(false);
            tutorialUI2.SetActive(false);
            clearUI.CloseUI();
            overUI.CloseUI();
        }
    }

    public void NextStage()
    {
        GameManager.Shop.isTutorial1 = false;
        int nextStageIndex = GameManager.Shop.stage.GetCurrentStageIndex() + 1;
        GameManager.Unit.CurrentStatReset();
        GameManager.SaveLoad.nextSceneIndex += nextStageIndex;
        GameManager.Stage.stageId++;
        GameManager.TurnBased.ChangeStartTurn();
        GameManager.SceneLoad.RestartScene();
    }

    private void MainmenuScene()
    {
        // 메인메뉴 (인트로?) 씬으로 
        GameManager.Shop.isTutorial1 = true;
        GameManager.ItemControl.drawPile.Clear();
        GameManager.TurnBased.turnSettingValue.isTutorial = false;
        GameManager.ItemControl.ClearData();
        GameManager.SceneLoad.LoadScene(SceneType.Title);
        GameManager.SaveLoad.nextSceneIndex = 0;
    }
}
