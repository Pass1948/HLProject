using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveBulletModalUI : BaseUI
{
    [SerializeField] private Transform ammoRoot;
    [SerializeField] private Button ammoItemPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button skipButton;

    private List<Ammo> _candidates;
    private int _selectedIndex = -1;
    private Action<int> _onConfirm;

    public void Open(List<Ammo> candidates, Action<int> onConfirm)
    {
        _candidates = candidates;
        _onConfirm = onConfirm;

        BuildList();

        confirmButton.interactable = false;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            _onConfirm?.Invoke(_selectedIndex);
        });

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(CloseUI);

        OpenUI();
    }

    private void BuildList()
    {
        Clear(ammoRoot);
        if (_candidates == null) return;

        for (int i = 0; i < _candidates.Count; i++)
        {
            int idx = i;
            var btn = Instantiate(ammoItemPrefab, ammoRoot);
            var txt = btn.GetComponentInChildren<Text>();
            if (txt) txt.text = _candidates[i].ToString();
            btn.onClick.AddListener(() => { _selectedIndex = idx; confirmButton.interactable = true; });
        }
    }

    private void Clear(Transform root)
    {
        if (!root) return;
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }
}