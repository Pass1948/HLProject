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

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        // TODO: 나중에 게임씬으로 바꿔주기
        //게임 씬 로드하는건 DeckSelUI에 옮겨놓고 DeckSelUI를 키게 작업해놓겠습니다
        //deckSelUI.SetActive(true);
        deckSelUI.SetActive(true);
    }
}
