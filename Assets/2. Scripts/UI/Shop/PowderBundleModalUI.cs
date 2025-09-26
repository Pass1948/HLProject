using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowderBundleModalUI : BaseUI
{

    [SerializeField] private Transform amooRoot;
    [SerializeField] private Transform powderRoot;
    [SerializeField] private Button ammoItemPrefab;
    [SerializeField] private Button powderItemPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button skipButton;

    private List<Ammo> _ammos;
    private List<PowderData> _powders;
    private int  _selectedAmmo = -1;
    private int _selectedPowder = -1;
    private Action<int,int> _onConfirm;

    public void Open(List<Ammo> ammos, List<PowderData> powders, Action<int,int> onConfirm)
    {
        _ammos = ammos;
        _powders = powders;
        _onConfirm = onConfirm;

        BuildAmmoList();
        BuildPowderList();
        confirmButton.interactable = false;
        
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            _onConfirm?.Invoke(_selectedAmmo, _selectedPowder);
        });

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() =>
        {
            CloseUI();
        });
        OpenUI();
    }

    private void BuildAmmoList()
    {
        Clear(amooRoot);
        for (int i = 0; i < _ammos.Count; i++)
        {
            int idx = i;
            var btn = Instantiate(ammoItemPrefab, amooRoot);
            btn.GetComponentInChildren<Text>().text = _ammos[i].ToString();
            btn.onClick.AddListener(() => { _selectedAmmo = idx; UpdateConfirm(); });
        }
    }

    private void BuildPowderList()
    {
        Clear(powderRoot);
        for (int i = 0; i < _powders.Count; i++)
        {
            int idx = i;
            var btn = Instantiate(powderItemPrefab, powderRoot);
            btn.GetComponentInChildren<Text>().text = _powders[i].name;
            btn.onClick.AddListener(() => { _selectedPowder = idx; UpdateConfirm(); });
        }
    }

    private void UpdateConfirm()
    {
        confirmButton.interactable = (_selectedAmmo >= 0 && _selectedPowder >= 0);
    }

    private void Clear(Transform root)
    {
        for (int i = root.childCount -1 ; i >= 0; i--) Destroy(root.GetChild(i).gameObject);
    }
}

