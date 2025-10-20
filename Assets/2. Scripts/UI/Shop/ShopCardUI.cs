
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI rankText2;
    [SerializeField] private TextMeshProUGUI suitText2;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image icon;
    public Button buyButton;

    public void Bind(ShopManager.ShopItem item)
    {
        rankText.text = RankLabel(item.ammo.rank);
        rankText2.text = RankLabel(item.ammo.rank);
        suitText.text = SuitLetter(item.ammo.suit);
        suitText2.text = SuitLetter(item.ammo.suit);
        priceText.text = item.price.ToString();
    }
    private static string SuitLetter(Suit s)
    {
        switch (s)
        {
            case Suit.Spade: return "♠";
            case Suit.Heart: return "♥";
            case Suit.Diamond: return "♦";
            case Suit.Club: return "♣";
            default: return "?";
        }
    }

    private static string RankLabel(int rank)
    {
        switch (rank)
        {
            case 1: return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return rank.ToString();
        }
    }
}
