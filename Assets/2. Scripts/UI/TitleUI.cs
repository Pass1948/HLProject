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

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        // TODO: 나중에 게임씬으로 바꿔주기
        GameManager.SceneLoad.LoadScene(SceneType.Test);
    }
}
