using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Analytics;
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

    //========[튜占썰리占쏙옙]========
    [SerializeField] GameObject tutorialUI1;
    [SerializeField] Button tutorialBtn1;
    [SerializeField] GameObject tutorialUI2;
    [SerializeField] Button tutorialBtn2;
   


    public ResultType resulttype;

   


    private void Start()
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

                tutorialUI1.SetActive(true);
                clearUI.CloseUI();
                overUI.CloseUI();
            }
            if(GameManager.Shop.isTutorial1 == false)
            {
                //TODO: tutorial_finish_menu_click
                Analytics.CustomEvent("tutorial_finish_menu_click", new Dictionary<string, object>
  {
    { "uiClick", "튜토리얼 2 메인 메뉴 버튼 클릭" },
  });
                tutorialUI1.SetActive(false);
                tutorialUI2.SetActive(true);
                clearUI.CloseUI();
                overUI.CloseUI();
            }
        }
        else if (result == ResultType.GameClear)
        {
            Analytics.CustomEvent("game_clear_popup", new Dictionary<string, object> // TODO : game_clear_popup
            {
                { "onScreen", "게임 클리어 팝업 출력" },
            });
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
        //TODO: tutorial1_next_click
        Analytics.CustomEvent("tutorial1_next_click", new Dictionary<string, object>
  {
    { "uiClick", "튜토리얼 1 다음 스테이지 버튼 클릭" },
  });
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
       
        GameManager.UI.OpenUI<FadeInUI>();
        GameManager.ItemControl.ClearData();
        GameManager.Unit.isRiding = false;
        GameManager.Shop.isTutorial1 = true;
        GameManager.TurnBased.turnSettingValue.isTutorial = false;
        GameManager.SceneLoad.LoadScene(SceneType.Title);
        GameManager.SaveLoad.nextSceneIndex = 0;
    }
}
