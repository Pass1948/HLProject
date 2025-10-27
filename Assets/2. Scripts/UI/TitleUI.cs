using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
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

    private AudioClip audioClip;

    private void Awake()
    {
        GameManager.Sound.PlayBGM(GameManager.Resource.Load<AudioClip>(Path.Sound + "BangPaladin"));
    }

    private void Start()
    {
        menuPanel.transform.DOMove(new Vector2(1300f,530f), 0.8f);
        logoPanel.transform.DOMove(new Vector2(0f,500f), 0.8f);
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OpenSetting);
        exitButton.onClick.AddListener(ExitButton);
        restartButton.onClick.AddListener(ReLoadPlay);
        tutorialYesBtn.onClick.AddListener(TutorialYes);
        tutorialNoBtn.onClick.AddListener(TutorialNo);
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
        deckSelUI.transform.DOMove(new Vector2(1300f,530f), 0.8f);
        GameManager.Sound.PlayUISfx();
        menuPanel.transform.DOMove(new Vector2(2300f,530f), 0.8f);
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
        settingUI.transform.DOMove(new Vector2(1400f,540f), 0.8f);
        menuPanel.transform.DOMove(new Vector2(2300f,530f), 0.8f);
        GameManager.Sound.PlayUISfx();
    }

    private void ReLoadPlay()
    {  
        GameManager.Sound.PlayUISfx();
    }

    private void ExitButton()
    {
#if UNITY_EDITOR
        
        Application.Quit();
        
#endif        

        GameManager.Sound.PlayUISfx();
    }

    private void ShowTutorialPopup()
    {
        deckSelUI.SetActive(false);
        tutorialPopup.SetActive(true);
    }

    private void TutorialYes()
    {
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
