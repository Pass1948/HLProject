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

    //오토바이
    [SerializeField] Button repairBtn;
    [SerializeField] Button mountBtn;
    [SerializeField] Button getOffBtn;

    // 테스트
    [SerializeField] Button test;

    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);
        rerollBtn.onClick.AddListener(OnReload);
        deckBtn.onClick.AddListener(DeckToggle);
        discardBtn.onClick.AddListener(DiscardToggle);
        repairBtn.onClick.AddListener(RepairButton);
        mountBtn.onClick.AddListener(OnRiding);
        getOffBtn.onClick.AddListener(GetOff);

        bikeControllBtn.onClick.AddListener(BikeToggle);
        atifactBtn.onClick.AddListener(AtifactToggle);

        // 테스트
        test.onClick.AddListener(BikeTest);


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

    private void RefreshVehicleUI()
    {
        switch (GameManager.Unit.Vehicle.vehicleModel.condition)
        {
            case VehicleCondition.Riding: // 위로 올라탔을 때
                rerollBtn.gameObject.SetActive(true);
                repairBtn.gameObject.SetActive(false);
                mountBtn.gameObject.SetActive(false);
                getOffBtn.gameObject.SetActive(true);
                break;
            case VehicleCondition.Repair: // 수리 하고 있을 때
                rerollBtn.gameObject.SetActive(false);
                repairBtn.gameObject.SetActive(true);
                mountBtn.gameObject.SetActive(false);
                getOffBtn.gameObject.SetActive(false);
                break;
            case VehicleCondition.Destruction: // 파괴 되었을 때
                if (IsNearVehicle()) // 가까이 있을 때 수리, 리롤
                {
                    rerollBtn.gameObject.SetActive(false);
                    repairBtn.gameObject.SetActive(true);
                    mountBtn.gameObject.SetActive(false);
                    getOffBtn.gameObject.SetActive(false);
                }

                else
                {
                    repairBtn.gameObject.SetActive(true);

                }
                    break;

            case VehicleCondition.GetOff: // 내렸을 때
                if (IsNearVehicle()) // 가까이 있을 때 수리, 리롤
                {
                    rerollBtn.gameObject.SetActive(true);
                    repairBtn.gameObject.SetActive(false);
                    mountBtn.gameObject.SetActive(true);
                    getOffBtn.gameObject.SetActive(false);
                }

                else
                {
                    rerollBtn.gameObject.SetActive(false);
                    mountBtn.gameObject.SetActive(false);
                }
                    break;

            default:
                rerollBtn.gameObject.SetActive(true);
                repairBtn.gameObject.SetActive(false);
                mountBtn.gameObject.SetActive(false);
                getOffBtn.gameObject.SetActive (true);
                break;
        }
    }

    private bool IsNearVehicle()
    {
        Vector3 vehiclePos = GameManager.Unit.Vehicle.transform.position;
        Vector3  playerPos = GameManager.Unit.Player.transform.position;
        Debug.Log($"{vehiclePos}{playerPos}");
        return playerPos == vehiclePos;
    }
    // 타는 로직
    private void OnRiding()
    {
        GameManager.Unit.Vehicle.vehicleHandler.MountVehicle();
    }
    // 내리는 로직
    private void GetOff()
    {
        GameManager.Unit.Vehicle.vehicleHandler.DismountVehicle();
    }
    // 수리 // 오토바이 버튼에 넣어주면 된다.
    private void RepairButton()
    {
        GameManager.Unit.Vehicle.vehicleHandler.RepairVehicle();
    }
    private void BikeToggle()
    {
        bikeControllBtnObj.ToggleBikeControll();
    }

    private void AtifactToggle()
    {
        atifactBtnObj.ToggleArtifactList();

    }

    // 테스트 버전
    private void BikeTest()
    {
        GameManager.Unit.Vehicle.vehicleHandler.DamageVehicle(3);
    }
}
