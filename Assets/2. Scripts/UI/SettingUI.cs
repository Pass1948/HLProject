using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button backToMainMenuBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(CloseSettingUi);
        backToMainMenuBtn.onClick.AddListener(BackToMainMenu);
    }

    private void CloseSettingUi()
    {
        gameObject.SetActive(false);
    }

    private void BackToMainMenu()
    {
        //여기에 메인메뉴가게 코드작성해야함
    }
}
