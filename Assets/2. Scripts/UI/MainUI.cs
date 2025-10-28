using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    [SerializeField] Button turnBtn;
    [SerializeField] AttackController fireBtnObj;

    [SerializeField] Button rerollBtn;
    [SerializeField] ReloadAmmo reloadBtnObj;

    [SerializeField] Button kickBtn;
    [SerializeField] ToggleBtnController kickBtnObj;

    [SerializeField] Button moveBtn;
    [SerializeField] ToggleBtnController moveBtnObj;

    //재장전 텍스트
    TMP_Text rerollLabel;

    [SerializeField] Button deckBtn;
    [SerializeField] ToggleBtnController deckBtnObj;

    [SerializeField] Button discardBtn;
    [SerializeField] ToggleBtnController discardBtnObj;

    [SerializeField] Button bikeControllBtn;
    [SerializeField] ToggleBtnController bikeControllBtnObj;

    [SerializeField] Button RelicBtn;
    [SerializeField] ToggleBtnController RelicBtnObj;

    [SerializeField] TMP_Text movementText;

    //오토바이
    [SerializeField] Button repairBtn;
    [SerializeField] Button mountBtn;
    [SerializeField] Button getOffBtn;

    //플레이어 HP
    [SerializeField] private Image playerHp;
    [SerializeField] private TMP_Text playerHpText;

    //바이크 HP
    [SerializeField] private Image vehicleHp;
    [SerializeField] private TMP_Text vehicleHpText;

    //설정버튼
    [SerializeField] Button settingActiveBtn;
    //설정창 UI
    [SerializeField] private GameObject settingUI;

    [SerializeField] private TextMeshProUGUI stageInfoText;

    //숫자가이드 버튼
    [SerializeField] Button guideBtn;
    [SerializeField] ToggleBtnController guideBtnObj;

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
        moveBtn.onClick.AddListener(ToggleMove);
        bikeControllBtn.onClick.AddListener(BikeToggle);
        RelicBtn.onClick.AddListener(RelicToggle);

        settingActiveBtn.onClick.AddListener(OpenSettings);

        guideBtn.onClick.AddListener(GuideToggle);
        //탄환 선택해제
        //오토바이 조작 안에 있는것들은 안넣어도 될것같음
        deckBtn.onClick.AddListener(fireBtnObj.Unselect);
        discardBtn.onClick.AddListener(fireBtnObj.Unselect);
        RelicBtn.onClick.AddListener(fireBtnObj.Unselect);
        turnBtn.onClick.AddListener(fireBtnObj.Unselect);
        bikeControllBtn.onClick.AddListener(fireBtnObj.Unselect);
        //kickBtn.onClick.AddListener(fireBtnObj.Unselect); //TODO: 진영님 나중에 이거 확인하세요(이영신)
        settingActiveBtn.onClick.AddListener(fireBtnObj.Unselect);

        InitReloadUI();
        SyncSettingManager();
    }

    private void OnEnable()
    {
        reloadBtnObj.ReloadChange += OnReloadChanged;
        SyncSettingManager();
        if (GameManager.TurnBased.turnSettingValue.isTutorial)
        {
            stageInfoText.text = $"Tutorial";
        }
        else
        {
            stageInfoText.text = $"Stage {(GameManager.SaveLoad.nextSceneIndex + 1).ToString()}";
        }
       
    }
    private void OnDisable()
    {
        reloadBtnObj.ReloadChange -= OnReloadChanged;
    }

    private void Update()
    {
        RefreshVehicleUI();
        RefreshMovement();
        RefreshPlayerHP();
        RefreshVehicleHP();

    }

    
    private void OnTurnEnd()
    {
        var t = turnBtn.transform.position;
        turnBtn.transform.DOMoveY(turnBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            turnBtn.transform.DOMoveY(turnBtn.transform.position.y + 5f, 0.1f);
            turnBtn.transform.position = t;
        });
        GameManager.TurnBased.ChangeTo<PlayerTurnEndState>("Force");
        GameManager.Sound.PlayUISfx();
    }

    private void OnReload()
    {
        reloadBtnObj.Reload();
        if (bikeControllBtnObj)
        {
            bikeControllBtnObj.ToggleBikeControll();
        }
        GameManager.Sound.PlayUISfx();
    }

    private void DeckToggle()
    {
        var t = deckBtn.transform.position;
        deckBtn.transform.DOMoveY(deckBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            deckBtn.transform.DOMoveY(deckBtn.transform.position.y + 5f, 0.1f);
            deckBtn.transform.position = t;
        });
        deckBtnObj.ToggleDeck();
        GameManager.Sound.PlayUISfx();
    }

    private void DiscardToggle()
    {
        var t = discardBtn.transform.position;
        discardBtn.transform.DOMoveY(discardBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            discardBtn.transform.DOMoveY(discardBtn.transform.position.y + 5f, 0.1f);
            discardBtn.transform.position = t;
        });
        discardBtnObj.ToggleDiscard();
        GameManager.Sound.PlayUISfx();
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
                if(CheckVehicleHp()) repairBtn.gameObject.SetActive(true);
                mountBtn.gameObject.SetActive(false);
                getOffBtn.gameObject.SetActive(false);
                break;
            case VehicleCondition.Destruction: // 파괴 되었을 때
                if (IsNearVehicle()) // 위치가 일치해 있을 때 수리, 리롤
                {
                    rerollBtn.gameObject.SetActive(true);
                    if(CheckVehicleHp()) repairBtn.gameObject.SetActive(true);
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
                    if(CheckVehicleHp()) repairBtn.gameObject.SetActive(true);
                    mountBtn.gameObject.SetActive(true);
                    getOffBtn.gameObject.SetActive(false);
                }
                else if(IsSideVehicle())// 주변에 있어도 
                    if(CheckVehicleHp()) repairBtn.gameObject.SetActive(true);
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
        var t = kickBtn.transform.position;
        kickBtn.transform.DOMoveY(kickBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            kickBtn.transform.DOMoveY(kickBtn.transform.position.y + 5f, 0.1f);
            kickBtn.transform.position = t;
        });
        GameManager.Mouse.HidePlayerRange();
        if (GameManager.Mouse.isShowRange == false) return;
        kickBtnObj.ToggleKick();
        GameManager.Sound.PlayUISfx();
    }


    void ToggleMove()
    {
        var t = moveBtn.transform.position;
        moveBtn.transform.DOMoveY(moveBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            moveBtn.transform.DOMoveY(moveBtn.transform.position.y + 5f, 0.1f);
            moveBtn.transform.position = t;
        });
        moveBtnObj.ToggleMove();
        GameManager.Sound.PlayUISfx();
    }
    // 오토바이와 플레이어의 위치가 겹치는지 쳌
    private bool IsNearVehicle()
    {
        Vector3 vehiclePos = GameManager.Unit.Vehicle.transform.position;
        Vector3  playerPos = GameManager.Unit.Player.transform.position;
        return playerPos == vehiclePos;
    }
    // 오토바이 체력 체크
    private bool CheckVehicleHp()
    {
        return GameManager.Unit.Vehicle.vehicleModel.currentHealth == 0 ? true : false;
    }
    // 오토바이와 플레이어의 위치값이 1이하인지
    private bool IsSideVehicle()
    {
        Vector3 vehiclePos = GameManager.Unit.Vehicle.transform.position;
        Vector3  playerPos = GameManager.Unit.Player.transform.position;
        return Vector3.Distance(vehiclePos, playerPos) >= 1f;
    }
    // 타는 로직
    private void OnRiding()
    {
        GameManager.Unit.Player.animHandler.OnRiding();
        GameManager.Unit.Vehicle.vehicleHandler.MountVehicle();
        if (bikeControllBtnObj)
        {
            bikeControllBtnObj.ToggleBikeControll();
        }
        GameManager.Sound.PlayUISfx();
    }
    // 내리는 로직
    private void GetOff()
    {
        GameManager.Unit.Player.animHandler.OnRiding();
        GameManager.Unit.Vehicle.vehicleHandler.DismountVehicle();
        if (bikeControllBtnObj)
        {
            bikeControllBtnObj.ToggleBikeControll();
        }
        GameManager.Sound.PlayUISfx();
    }
    // 수리 // 오토바이 버튼에 넣어주면 된다.
    private void RepairButton()
    {
        var t = repairBtn.transform.position;
        repairBtn.transform.DOMoveY(repairBtn.transform.position.y - 2f, 0.1f).OnComplete(() =>
        {
            repairBtn.transform.DOMoveY(repairBtn.transform.position.y + 2f, 0.1f);
            repairBtn.transform.position = t;
        });
        GameManager.Unit.Vehicle.vehicleHandler.RepairVehicle();
        if (bikeControllBtnObj)
        {
            bikeControllBtnObj.ToggleBikeControll();
        }
        GameManager.Sound.PlayUISfx();
        GameManager.TurnBased.ChangeTo<PlayerTurnEndState>("Force");
    }
    private void BikeToggle()
    {
        var t = bikeControllBtn.transform.position;
        bikeControllBtn.transform.DOMoveY(bikeControllBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            bikeControllBtn.transform.DOMoveY(bikeControllBtn.transform.position.y + 5f, 0.1f);
            bikeControllBtn.transform.position = t;
        });
        bikeControllBtnObj.ToggleBikeControll();
        GameManager.Sound.PlayUISfx();
    }

    private void RelicToggle()
    {
        var t = RelicBtn.transform.position;
        RelicBtn.transform.DOMoveY(RelicBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            RelicBtn.transform.DOMoveY(RelicBtn.transform.position.y + 5f, 0.1f);
            RelicBtn.transform.position = t;
        });
        RelicBtnObj.ToggleRelicList();
        GameManager.Sound.PlayUISfx();
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
       
       if(remain <= 0)
       {
            rerollBtn.interactable = false;
            return;
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
        movementText.text = GameManager.Unit.Player.playerModel.moveRange.ToString();
    }

    private void RefreshPlayerHP()
    {
        var m = GameManager.Unit.Player.playerModel;

        int cur = m.currentHealth;
        int max = m.maxHealth;

        float t = (max > 0) ? (float)cur / max : 0f;
        if (playerHp) playerHp.fillAmount = Mathf.Clamp01(t);
        if (playerHpText) playerHpText.text = $"{cur}/{max}";
    }

    private void RefreshVehicleHP()
    {
        var m = GameManager.Unit.Vehicle.vehicleModel;

        int cur = m.currentHealth;
        int max = m.maxHealth;

        float t = (max > 0) ? (float)cur / max : 0f;
        if (vehicleHp) vehicleHp.fillAmount = Mathf.Clamp01(t);
        if (vehicleHpText) vehicleHpText.text = $"{cur}/{max}";
    }

    private void OpenSettings()
    {
        var t = settingActiveBtn.transform.position;
        settingActiveBtn.transform.DOMoveY(settingActiveBtn.transform.position.y - 5f, 0.1f).OnComplete(() =>
        {
            settingActiveBtn.transform.DOMoveY(settingActiveBtn.transform.position.y + 5f, 0.1f);
            settingActiveBtn.transform.position = t;
        });
        settingUI.transform.DOLocalMove(new Vector2(0, 0), 0.8f);
        GameManager.Sound.PlayUISfx();
    }

    //숫자 가이드 패널 열렸다 닫혔다
    private void GuideToggle()
    {
        guideBtnObj.ToggleGuide();
    }

    //가이드 버튼이 보이기/숨기기되게
    public bool GuideVisible
    {
        get => guideBtn && guideBtn.gameObject.activeSelf;
        set { if (guideBtn) guideBtn.gameObject.SetActive(value); }
    }

    private void SyncSettingManager()
    {
        GuideVisible = GameManager.UI?.ShowGuide ?? true;
    }
}
