using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Transform windowPanel;
    [SerializeField] private Button windowPrevBtn;
    [SerializeField] private Button windowNextBtn;

    private const string windowPanelPrefix = "WindowSizePanel_";
    private int windowPanelIndex = 0;
    private GameObject[] windowPanels;

    [SerializeField] Button closeBtn;
    [SerializeField] Button backToMainMenuBtn;

    [SerializeField] private Transform speedPanel;
    [SerializeField] private Button speedPrevBtn;
    [SerializeField] private Button speedNextBtn;

    private const string SpeedPrefix = "SpeedPanel_";
    private int speedIndex = 0;
    private GameObject[] speedPanels;

    //숫자 가이드 토글
    [SerializeField] private Toggle showGuideToggle;

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

        showGuideToggle.onValueChanged.AddListener(GuideToggleChanged);
        AutoRegisterGuide();
    }
    private void OnDestroy()
    {
        showGuideToggle?.onValueChanged.RemoveListener(GuideToggleChanged);
    }

    private void CloseSettingUi()
    {
        gameObject.SetActive(false);
    }

    private void BackToMainMenu()
    {
        Debug.Log("메인메뉴로 가기");
        GameManager.SceneLoad.LoadScene(SceneType.Title);
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
        windowPanelIndex = (windowPanelIndex - 1 + windowPanels.Length) % windowPanels.Length;
        UpdateWindowView();
    }

    void WindowNextPanel()
    {
        if (windowPanels == null || windowPanels.Length == 0) return;
        windowPanelIndex = (windowPanelIndex + 1) % windowPanels.Length;
        UpdateWindowView();
    }

    private void UpdateWindowView()
    {
        if (windowPanels == null) return;
        for (int i = 0; i < windowPanels.Length; i++)
            windowPanels[i].SetActive(i == windowPanelIndex);
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
        speedIndex = (speedIndex - 1 + speedPanels.Length) % speedPanels.Length;
        UpdateSpeedView();
        
    }

    void SpeedNextPanel()
    {
        if (speedPanels == null || speedPanels.Length == 0) return;
        speedIndex = (speedIndex + 1) % speedPanels.Length;
        UpdateSpeedView();
    }

    private void UpdateSpeedView()
    {
        if (speedPanels == null) return;
        for (int i = 0; i < speedPanels.Length; i++)
            speedPanels[i].SetActive(i == speedIndex);
    }

    // === Guide 섹션 ===
    void AutoRegisterGuide()
    {
        // 저장된 값으로 토글 초기 동기화
        bool on = GameManager.UI?.ShowGuide ?? true;
        showGuideToggle.SetIsOnWithoutNotify(on);
        ApplyGuide(on); // 열릴 때 한 번 즉시 반영
    }

    void GuideToggleChanged(bool on)
    {
        // 토글 바뀌자마자 저장 + 즉시 반영
        ApplyGuide(on);
    }

    // 공통 처리: 값 저장(씬 간 유지) + 씬에 MainUI 있으면 즉시 반영
    void ApplyGuide(bool on)
    {
        if (GameManager.UI) GameManager.UI.ShowGuide = on;

        var main = FindFirstObjectByType<MainUI>(FindObjectsInactive.Include);
        if (main) main.GuideVisible = on;
    }
}
