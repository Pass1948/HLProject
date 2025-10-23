using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : PopUpUI
{
    [SerializeField] private Transform windowPanel;
    [SerializeField] private Button windowPrevBtn;
    [SerializeField] private Button windowNextBtn;

    private const string windowPanelPrefix = "WindowSizePanel_";

    private GameObject[] windowPanels;

    [SerializeField] Button closeBtn;
    [SerializeField] Button backToMainMenuBtn;

    [SerializeField] private Transform speedPanel;
    [SerializeField] private Button speedPrevBtn;
    [SerializeField] private Button speedNextBtn;
    
    [SerializeField] private Slider masterVolumeBar;
    [SerializeField] private Slider bgmVolumeBar;
    [SerializeField] private Slider sfxVolumeBar;
    

    private const string SpeedPrefix = "SpeedPanel_";
    private int speedIndex = 0;
    private GameObject[] speedPanels;

    private void Awake()
    {
        windowPrevBtn.onClick.AddListener(WindowPrevPanel);
        windowNextBtn.onClick.AddListener(WindowNextPanel);
        AutoRegisterWindowPanels();
        UpdateWindowView();

        speedPrevBtn.onClick.AddListener(SpeedPrevPanel);
        speedNextBtn.onClick.AddListener(SpeedNextPanel);
        AutoRegisterSpeedPanels();
        UpdateSpeedView();

        closeBtn.onClick.AddListener(CloseSettingUi);
        backToMainMenuBtn.onClick.AddListener(BackToMainMenu);
        
        masterVolumeBar.minValue = 0f; masterVolumeBar.maxValue = 1f;
        bgmVolumeBar.minValue = 0f; bgmVolumeBar.maxValue = 1f;
        sfxVolumeBar.minValue = 0f; sfxVolumeBar.maxValue = 1f;
    }

    private void OnEnable()
    {
        UpdateSpeedView();
        masterVolumeBar.onValueChanged.AddListener(GameManager.Sound.SetMasterVolume);
        sfxVolumeBar.onValueChanged.AddListener(GameManager.Sound.SetSfxVolume);
        bgmVolumeBar.onValueChanged.AddListener(GameManager.Sound.SetBgmVolume);
        masterVolumeBar.value = GameManager.Sound.masterVolume;
        sfxVolumeBar.value = GameManager.Sound.sfxVolume;
        bgmVolumeBar.value = GameManager.Sound.bgmVolume;
    }

    private void OnDisable()
    {
        masterVolumeBar.onValueChanged.RemoveListener(GameManager.Sound.SetMasterVolume);
        sfxVolumeBar.onValueChanged.RemoveListener(GameManager.Sound.SetSfxVolume);
        bgmVolumeBar.onValueChanged.RemoveListener(GameManager.Sound.SetBgmVolume);
    }


    private void CloseSettingUi()
    {
        CloseUI();
        GameManager.Sound.PlayUISfx();
    }

    private void BackToMainMenu()
    {
        Debug.Log("메인메뉴로 가기");
        GameManager.SceneLoad.LoadScene(SceneType.Title);
        GameManager.Sound.PlayUISfx();
    }

    void AutoRegisterWindowPanels()
    {
        var root = windowPanel ? windowPanel : transform;
        var list = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            if (c.name.StartsWith(windowPanelPrefix)) list.Add(c.gameObject);
        }
        windowPanels = list.ToArray();
    }

    void WindowPrevPanel()
    {
        GameManager.TurnBased.turnSettingValue.windowPanelIndex-= 1;
        if(GameManager.TurnBased.turnSettingValue.windowPanelIndex < 0)
            GameManager.TurnBased.turnSettingValue.windowPanelIndex = windowPanels.Length;
        UpdateWindowView();
        GameManager.Sound.PlayUISfx();
    }

    void WindowNextPanel()
    {
        GameManager.TurnBased.turnSettingValue.windowPanelIndex += 1;
        if (GameManager.TurnBased.turnSettingValue.windowPanelIndex > 2)
            GameManager.TurnBased.turnSettingValue.windowPanelIndex = 0;
        UpdateWindowView();
        GameManager.Sound.PlayUISfx();
    }

    private void UpdateWindowView()
    {
        var index = GameManager.TurnBased.turnSettingValue.windowPanelIndex;

        if (windowPanels == null) return;
        for (int i = 0; i < windowPanels.Length; i++)
            windowPanels[i].SetActive(i == index);
        switch (index)
        {
            case 0:
                Screen.fullScreen = true;
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case 1:
                Screen.fullScreen = false;
                Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
                break;
            case 2:
                Screen.fullScreen = false;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;
        }
    }

    void AutoRegisterSpeedPanels()
    {
        var root = speedPanel ? speedPanel : transform;
        var list = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            if (c.name.StartsWith(SpeedPrefix)) list.Add(c.gameObject);
        }
        speedPanels = list.ToArray();
    }

    void SpeedPrevPanel()
    {
        var t = GameManager.TurnBased.turnSettingValue.gameTime-=1;
        if(GameManager.TurnBased.turnSettingValue.gameTime < 1)
            GameManager.TurnBased.turnSettingValue.gameTime = 4;
        Time.timeScale = t;
        UpdateSpeedView();
        GameManager.Sound.PlayUISfx();
    }

    void SpeedNextPanel()
    {
        var t = GameManager.TurnBased.turnSettingValue.gameTime += 1;
        if (GameManager.TurnBased.turnSettingValue.gameTime >4)
            GameManager.TurnBased.turnSettingValue.gameTime = 1;
        Time.timeScale = t;
        UpdateSpeedView();
        GameManager.Sound.PlayUISfx();
    }

    private void UpdateSpeedView()
    {
        int t = GameManager.TurnBased.turnSettingValue.gameTime;
        if (speedPanels == null) return;
        for (int i = 0; i < speedPanels.Length; i++)
            speedPanels[i].SetActive(i == t - 1);
        GameManager.Sound.PlayUISfx();
    }
}
