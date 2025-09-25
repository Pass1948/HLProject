using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : BaseUI
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text priceText;
    [SerializeField] private Image icon;
    public Button buyButton;

    public void Bind(ShopManager.ShopItem item)
    {
        titleText.text = item.name;
        priceText.text = item.price.ToString();
    }
}
