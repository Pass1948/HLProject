using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : BaseUI
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject deckSelUI;
    [SerializeField] private GameObject settingUI;

    private AudioClip audioClip;

    private void Awake()
    {
        GameManager.Sound.PlayBGM(GameManager.Resource.Load<AudioClip>(Path.Sound + "Paladin’s Dash"));
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OpenSetting);
        exitButton.onClick.AddListener(ExitButton);
        restartButton.onClick.AddListener(ReLoadPlay);
    }

    private void StartGame()
    {
        // TODO: 나중에 게임씬으로 바꿔주기
        //게임 씬 로드하는건 DeckSelUI에 옮겨놓고 DeckSelUI를 키게 작업해놓겠습니다
        //deckSelUI.SetActive(true);
        deckSelUI.SetActive(true);
        GameManager.Sound.PlayUISfx();
    }

    private void OpenSetting()
    {
        GameManager.UI.OpenPopUI<SettingUI>();
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
}
