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

    [SerializeField] Button repairBtn;


    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);
        rerollBtn.onClick.AddListener(OnReload);
        deckBtn.onClick.AddListener(DeckToggle);
        discardBtn.onClick.AddListener(DiscardToggle);
        repairBtn.onClick.AddListener(RepairButton);

        //시작시에 한번 실행되게
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
    }

    private void Update()
    {
        //선택 없으면 버튼 비활성하기 <- 이러면 리스너 실행안됨
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
        RefreshVehicleUI();
    }

    private void RefreshVehicleUI()
    {
        switch (GameManager.Unit.Vehicle.vehicleModel.condition)
        {
            case VehicleCondition.Bording: // 위로 올라탔을 때
                rerollBtn.gameObject.SetActive(true);
                repairBtn.gameObject.SetActive(false);
                break;
            case VehicleCondition.Repair: // 수리 하고 있을 때
                rerollBtn.gameObject.SetActive(true);
                repairBtn.gameObject.SetActive(false);
                break;
            case VehicleCondition.Destruction: // 파괴 되었을 때
                rerollBtn.gameObject.SetActive(false);
                repairBtn.gameObject.SetActive(true);
                break;
            default:
                rerollBtn.gameObject.SetActive(false);
                break;
        }
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
    private void RepairButton()
    {
        GameManager.Unit.Vehicle.vehicleHandler.RepairVehicle();
    }
}
