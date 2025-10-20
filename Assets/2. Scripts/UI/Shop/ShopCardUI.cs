
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : BaseUI
{
    private static ShopCardUI currentSelectedCard;
    [SerializeField] private GameObject shopCardUI;
    [SerializeField] private GameObject buyCardUI;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI rankText2;
    [SerializeField] private TextMeshProUGUI suitText2;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button bulltBtn;
    [SerializeField] public Button playerCardBtn;
 public Button buyButton;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        bulltBtn.onClick.AddListener(OnBulltClick);
    }


    private void OnBulltClick()
    {
        if (currentSelectedCard != null && currentSelectedCard != this)
        {
            currentSelectedCard.animator.SetBool("IsOnClick", false);
        }
        animator.SetBool("IsOnClick", true);
        currentSelectedCard = this;
    }

    public void OnPlayerCard()
    {
        if (currentSelectedCard != null && currentSelectedCard != this)
        {
            currentSelectedCard.animator.SetBool("IsOnPlayerCardClick", false);
        }
        animator.SetBool("IsOnPlayerCardClick", true);
        currentSelectedCard = this;
    }

    public void OnBuyCard()
    {
        shopCardUI.SetActive(false);
        buyCardUI.SetActive(true);
    }


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
