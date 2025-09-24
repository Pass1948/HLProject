using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUI : BaseUI
{
    [SerializeField] Button turnBtn;
    [SerializeField] AttackController fireBtnObj;

    [SerializeField] Button rerollBtn;
    [SerializeField] ReloadAmmo reloadBtnObj;

    [SerializeField] Button kickBtn;
    [SerializeField] ToggleBtnController kickBtnObj;

    //재장전 텍스트
    TMP_Text rerollLabel;

    [SerializeField] Button deckBtn;
    [SerializeField] ToggleBtnController deckBtnObj;

    [SerializeField] Button discardBtn;
    [SerializeField] ToggleBtnController discardBtnObj;

    [SerializeField] Button bikeControllBtn;
    [SerializeField] ToggleBtnController bikeControllBtnObj;

    [SerializeField] Button atifactBtn;
    [SerializeField] ToggleBtnController atifactBtnObj;

    [SerializeField] TMP_Text movementText;

    //오토바이
    [SerializeField] Button repairBtn;
    [SerializeField] Button mountBtn;
    [SerializeField] Button getOffBtn;
    [SerializeField] Button settingBtn; 

    private void Awake()
    {
        turnBtn.onClick.AddListener(OnTurnEnd);
        rerollBtn.onClick.AddListener(OnReload);
        deckBtn.onClick.AddListener(DeckToggle);
        discardBtn.onClick.AddListener(DiscardToggle);
        repairBtn.onClick.AddListener(RepairButton);
        mountBtn.onClick.AddListener(OnRiding);
        getOffBtn.onClick.AddListener(GetOff);
        kickBtn.onClick.AddListener(ToggleKick);

        bikeControllBtn.onClick.AddListener(BikeToggle);
        atifactBtn.onClick.AddListener(AtifactToggle);


        settingBtn.onClick.AddListener(GameResultUITest);

        InitReloadUI();
    }

    private void OnEnable()
    {
        reloadBtnObj.ReloadChange += OnReloadChanged;
    }
    private void OnDisable()
    {
        reloadBtnObj.ReloadChange -= OnReloadChanged;
    }

    private void GameResultUITest()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        //backUI.resulttype = ResultType.Over;
        backUI.GetResultType(ResultType.Over);
    }

    private void Update()
    {
        RefreshVehicleUI();
        RefreshMovement();
    }

    
    private void OnTurnEnd()
    {
        GameManager.TurnBased.ChangeTo<PlayerTurnEndState>("Force");
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

    // ReSharper disable Unity.PerformanceAnalysis
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
                if (IsNearVehicle()) // 위치가 일치해 있을 때 수리, 리롤
                {
                    rerollBtn.gameObject.SetActive(true);
                    repairBtn.gameObject.SetActive(true);
                    mountBtn.gameObject.SetActive(false);
                    getOffBtn.gameObject.SetActive(false);
                }
                else if(IsSideVehicle())// 주변에 있어도 
                    rerollBtn.gameObject.SetActive(true);
                else
                {
                    repairBtn.gameObject.SetActive(false);
                }
                break;

            case VehicleCondition.GetOff: // 내렸을 때
                if (IsNearVehicle()) // 위치가 일치해 있을 때 수리,
                {
                    rerollBtn.gameObject.SetActive(true);
                    repairBtn.gameObject.SetActive(true);
                    mountBtn.gameObject.SetActive(true);
                    getOffBtn.gameObject.SetActive(false);
                }
                else if(IsSideVehicle())// 주변에 있어도 
                    rerollBtn.gameObject.SetActive(true);
                else
                {
                    rerollBtn.gameObject.SetActive(false);
                    mountBtn.gameObject.SetActive(false);
                    repairBtn.gameObject.SetActive(false);
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

    void ToggleKick()
    {
        kickBtnObj.ToggleKick();
    }

    private bool IsNearVehicle()
    {
        Vector3 vehiclePos = GameManager.Unit.Vehicle.transform.position;
        Vector3  playerPos = GameManager.Unit.Player.transform.position;
        return playerPos == vehiclePos;
    }

    private bool IsSideVehicle()
    {
        Vector3 vehiclePos = GameManager.Unit.Vehicle.transform.position;
        Vector3  playerPos = GameManager.Unit.Player.transform.position;
        return Vector3.Distance(vehiclePos, playerPos) >= 1f;
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
        GameManager.TurnBased.ChangeTo<PlayerTurnEndState>("Force");
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

    
    private void OnReloadChanged(int remain, int max)
    {
        ApplyReloadUI(remain, max);
    }

    //재장전 텍스트 적용 텍스트에선 남은 횟수만 보이게
    private void ApplyReloadUI(int remain, int max)
    {
       if( (remain > 0) || rerollBtn.gameObject.activeInHierarchy)
       {
            rerollBtn.interactable = true;
       }
       else
       {
            rerollBtn.interactable = false;
       }
            
       if (!rerollLabel && rerollBtn)
       {
            rerollLabel = rerollBtn.GetComponentInChildren<TMP_Text>(true);
       }

       if (rerollLabel)
       {
            rerollLabel.text = $"재장전 x{remain}";
       }    
    }

    private void InitReloadUI()
    {
        //라벨 참조
        if (!rerollLabel && rerollBtn)
        {
            rerollLabel = rerollBtn.GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void RefreshMovement()
    {
        int move = 0;
        if(GameManager.Map != null)
        {
            move = GameManager.Map.moveRange;
        }
        movementText.text = ($"Movement: {move}");
    }
}
