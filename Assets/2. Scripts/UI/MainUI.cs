using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    [SerializeField] Button fireBtn;
    [SerializeField] AttackController fireBtnObj;

    [SerializeField] private Button reloadBtn;
    [SerializeField] private ReloadAmmo reloadBtnObj;

    private void Awake()
    {
        fireBtn.onClick.AddListener(OnFire);
        reloadBtn.onClick.AddListener(OnReload);
    }

    private void OnFire()
    {
        fireBtnObj.Fire();
    }

    private void OnReload()
    {
        reloadBtnObj.Reload();
    }



}
