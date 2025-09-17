using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    [SerializeField] Button fireBtn;
    [SerializeField] AttackController fireBtnObj;

    [SerializeField] Button rerollBtn;
    [SerializeField] ReloadAmmo reloadBtnObj;

    [SerializeField] Button deckBtn;
    [SerializeField] DeckDiscardOnOff deckBtnObj;

    [SerializeField] Button discardBtn;
    [SerializeField] DeckDiscardOnOff discardBtnObj;


    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);
        rerollBtn.onClick.AddListener(OnReload);
        deckBtn.onClick.AddListener(DeckToggle);
        discardBtn.onClick.AddListener(DiscardToggle);

        //시작시에 한번 실행되게
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
    }

    private void Update()
    {
        //선택 없으면 버튼 비활성하기 <- 이러면 리스너 실행안됨
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
    }

    private void OnFire()
    {
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Attack);
        fireBtnObj.Fire();
    }

    private void OnReload()
    {
        reloadBtnObj.Reload();
    }

    private void DeckToggle()
    {
        deckBtnObj.ToggleDeck();
    }

    private void DiscardToggle()
    {
        discardBtnObj.ToggleDiscard();
    }

}
