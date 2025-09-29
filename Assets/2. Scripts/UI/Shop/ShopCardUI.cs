using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image icon;
    public Button buyButton;

    public void Bind(ShopManager.ShopItem item)
    {
        titleText.text = item.name;
        priceText.text = item.price.ToString();
    }
}
