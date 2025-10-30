using DG.Tweening;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

static class TutorialSave
{
    public static bool IsTutorial = true;
}

public class TitleUI : BaseUI
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject deckSelUI;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private RectTransform logoPanel;
    
    //튜토리얼 관련
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private Button tutorialYesBtn;
    [SerializeField] private Button tutorialNoBtn;

    private void Awake()
    {
        GameManager.Sound.PlayBGM(GameManager.Resource.Load<AudioClip>(Path.Sound + "BangPaladin"));
    }



    private void Start()
    {
        menuPanel.transform.DOLocalMove(new Vector2(300, -24.92419f), 0.8f);
        logoPanel.transform.DOLocalMove(new Vector2(-960, -55.92401f), 0.8f);
    }
    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OpenSetting);
        exitButton.onClick.AddListener(ExitButton);
        restartButton.onClick.AddListener(ReLoadPlay);
        tutorialYesBtn.onClick.AddListener(TutorialYes);
        tutorialNoBtn.onClick.AddListener(TutorialNo);
        //TODO: title_enter
        Analytics.CustomEvent("title_enter", new Dictionary<string, object>
  {
    { "onScreen", "타이틀 진입" },
  });
    }
    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartGame);
        settingButton.onClick.RemoveListener(OpenSetting);
        exitButton.onClick.RemoveListener(ExitButton);
        restartButton.onClick.RemoveListener(ReLoadPlay);
    }

    private void StartGame()
    {
        Analytics.CustomEvent("title_new_game_click", new Dictionary<string, object> // TODO: title_new_game_click
        {
            { "uiClick", "타이틀 화면에서 새로 시작 버튼 클릭" },
        });

        deckSelUI.transform.DOLocalMove(new Vector2(0, 0), 0.8f);
        GameManager.Sound.PlayUISfx();
        menuPanel.transform.DOLocalMove(new Vector2(2400, -24.92419f), 0.8f);
        GameManager.Sound.PlayUISfx();
        if(TutorialSave.IsTutorial)
        {
            ShowTutorialPopup();
            TutorialSave.IsTutorial = false;
            return;
        }
        

        deckSelUI.SetActive(true);
    }

    private void OpenSetting()
    {
        settingUI.transform.DOLocalMove(new Vector2(0, 0), 0.8f);
        menuPanel.transform.DOLocalMove(new Vector2(2400, -24.92419f), 0.8f);
        GameManager.Sound.PlayUISfx();
    }

    private void ReLoadPlay()
    {  
        GameManager.Sound.PlayUISfx();
    }

    private void ExitButton()
    {
#if UNITY_EDITOR
        AnalyticsService.Instance.StopDataCollection();
        Application.Quit();
        
#endif        

        GameManager.Sound.PlayUISfx();
    }

    private void ShowTutorialPopup()
    {
        //TODO: tutorial_popup_show
        Analytics.CustomEvent("tutorial_popup_show", new Dictionary<string, object>
  {
    { "onScreen", "튜토리얼창열림" },
  });
        deckSelUI.SetActive(false);
        tutorialPopup.SetActive(true);
    }

    private void TutorialYes()
    {
        //TODO: tutorial_popup_yes
        Analytics.CustomEvent("tutorial_popup_yes", new Dictionary<string, object>
  {
    { "uiClick", "튜토리얼YES" },
  });
        //여기에 튜토리얼 스테이지 진입넣으면 됩니다
        GameManager.TurnBased.turnSettingValue.isTutorial = true;
        GameManager.UI.OpenUI<FadeInUI>();
        GameManager.Map.TutorialStage();
        GameManager.SceneLoad.LoadScene(SceneType.Test);
    }

    private void TutorialNo()
    {
        tutorialPopup.SetActive(false);
        deckSelUI.SetActive(true);
    }
}
