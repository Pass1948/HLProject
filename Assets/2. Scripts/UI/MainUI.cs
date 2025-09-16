using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    [SerializeField] Button fireBtn;
    [SerializeField] AttackController fireBtnObj;

    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);
    }

    private void OnFire()
    {
        GameManager.TurnBased.SetSelectedAction(PlayerActionType.Attack);
        fireBtnObj.Fire();
    }




}
