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
    [SerializeField] ToggleBtnController deckBtnObj;

    [SerializeField] Button discardBtn;
    [SerializeField] ToggleBtnController discardBtnObj;

    [SerializeField] Button bikeControllBtn;
    [SerializeField] ToggleBtnController bikeControllBtnObj;

    [SerializeField] Button atifactBtn;
    [SerializeField] ToggleBtnController atifactBtnObj;

    [SerializeField] Button repairBtn;


    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);
        rerollBtn.onClick.AddListener(OnReload);
        deckBtn.onClick.AddListener(DeckToggle);
        discardBtn.onClick.AddListener(DiscardToggle);
        repairBtn.onClick.AddListener(RepairButton);

        bikeControllBtn.onClick.AddListener(BikeToggle);
        atifactBtn.onClick.AddListener(AtifactToggle);


        //시작시에 한번 실행되게
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
    }

    private void Update()
    {
        //선택 없으면 버튼 비활성하기 <- 이러면 리스너 실행안됨
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
        RefreshVehicleUI();
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
    // 수리수리 마수리 // 오토바이 버튼에 넣어주면 된다.
    private void RepairButton()
    {
        GameManager.Unit.Vehicle.vehicleHandler.RepairVehicle();
    }
    private void RefreshVehicleUI()
    {
        switch (GameManager.Unit.Vehicle.vehicleModel.condition)
        {
            case VehicleCondition.Riding: // 위로 올라탔을 때
                rerollBtn.gameObject.SetActive(true);
                repairBtn.gameObject.SetActive(false);
                break;
            case VehicleCondition.Repair: // 수리 하고 있을 때
                rerollBtn.gameObject.SetActive(true);
                repairBtn.gameObject.SetActive(false);
                break;
            case VehicleCondition.Destruction: // 파괴 되었을 때
                rerollBtn.gameObject.SetActive(false);

                if (IsNearVehicle()) // 가까이 있을 때.
                    repairBtn.gameObject.SetActive(true);
                break;
            default:
                rerollBtn.gameObject.SetActive(false);
                break;
        }
    }

    private bool IsNearVehicle()
    {
        var vehiclePos = GameManager.Unit.Vehicle.transform.position;
        var playerPos = GameManager.Unit.Player.transform.position;

        return Vector3.Distance(playerPos, vehiclePos) < 1.5f;
    }

    private void BikeToggle()
    {
        bikeControllBtnObj.ToggleBikeControll();
    }

    private void AtifactToggle()
    {
        atifactBtnObj.ToggleArtifactList();

    }
}
