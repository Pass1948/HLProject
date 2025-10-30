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

    //========[Ʃ�丮��]========
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
            Analytics.CustomEvent("game_clear_popup", new Dictionary<string, object>//TODO : game_clear_popup
            {
                { "///", "///" },
            });
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
                Debug.Log("Ʃ�丮�� 1");
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
        // ���θ޴� (��Ʈ��?) ������ 
        GameManager.UI.OpenUI<FadeInUI>();
        GameManager.ItemControl.ClearData();
        GameManager.Unit.isRiding = false;
        GameManager.Shop.isTutorial1 = true;
        GameManager.TurnBased.turnSettingValue.isTutorial = false;
        GameManager.SceneLoad.LoadScene(SceneType.Title);
        GameManager.SaveLoad.nextSceneIndex = 0;
    }
}
