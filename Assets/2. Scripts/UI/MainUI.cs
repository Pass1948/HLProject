using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    [SerializeField] Button fireBtn;
    [SerializeField] AttackController fireBtnObj;

    [SerializeField] private Button rerollBtn;
    [SerializeField] private ReloadAmmo reloadBtnObj;


    [SerializeField] private Button kickBtn;

    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);    // 공격버튼
        rerollBtn.onClick.AddListener(OnReload);    // 재장전 버튼
       // kickBtn.onClick.AddListener(OnKick);    // 킥 버튼 나중에 추가

        //시작시에 한번 실행되게
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
    }

    private void Update()
    {
        //선택 없으면 버튼 비활성하기 <- 이러면 리스너 실행안됨
        fireBtn.interactable = (fireBtnObj != null) && fireBtnObj.IsBtnSel;
    }

    // 공격 버튼 클릭 처리
    private void OnFire()
    {
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Attack);
         fireBtnObj.Fire(); // TODO: 이후 공격범위나 몬스터를 클릭시 공격실행하도록 변경예정
    }
    // 리로드 버튼 클릭 처리
    private void OnReload()
    {
        reloadBtnObj.Reload();
    }

    // 킥 버튼 클릭 처리
    private void OnKick()
    {
        // TODO: 이후 킥범위나 몬스터를 클릭시 킥실행하도록 변경예정
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Kick);
    }
}
