
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : BaseUI
{
    public Button bulletBtn;
    public Button rellicBtn;
    public Button buyBulletBtn;
    Animator animator;
    private static ShopCardUI currentSelectedCard;

    [Header("=====탄환=====")]
    [SerializeField] private GameObject bulletUI;

    [Header("상점탄환")]
    [SerializeField] private GameObject shopCardUI;
    [SerializeField] public GameObject buyCardBtn;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button shopBulltBtn;

    [Header("구매탄환")]
    [SerializeField] private GameObject buyCardUI;
    [SerializeField] private TextMeshProUGUI rankText2;
    [SerializeField] private TextMeshProUGUI suitText2;
    [SerializeField] private Image buyBulletBG1;
    [SerializeField] private Image buyBulletBG2;


    [Header("=====유물=====")]
    [SerializeField] private GameObject rellicUI;
    [SerializeField] private TextMeshProUGUI rellicName;
    [SerializeField] private TextMeshProUGUI rellicDesc;

    [Header("상점유물")]
    [SerializeField] private GameObject shopRellicUI;
    [SerializeField] private GameObject shopRellicBuyBtn;
    [SerializeField] private TextMeshProUGUI rellicPrice;
    [SerializeField] public Button shopRellicItemBtn;

    [Header("구매유물")]
    [SerializeField] private GameObject buyRellicUI;
    [SerializeField] private Button buyRelletBtn;
    [SerializeField] private TextMeshProUGUI rellicName2;
    [SerializeField] private TextMeshProUGUI rellicDesc2;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        shopBulltBtn.onClick.AddListener(OnBulltClick);
        shopRellicItemBtn.onClick.AddListener(OnRellicClick);
    }

    // ===========[탄환 애니메이션]=============
    private void OnBulltClick()
    {
        if (currentSelectedCard != null && currentSelectedCard != this)
        {
            currentSelectedCard.animator.SetBool("IsClickShopBulletBtn", false);
        }
        if (currentSelectedCard == this)
        {
            animator.SetBool("IsClickShopBulletBtn", false);
            currentSelectedCard = null;
            return;
        }
        animator.SetBool("IsClickShopBulletBtn", true);
        currentSelectedCard = this;
    }

    public void OnPlayerCard(GameObject remove)
    {
        if (currentSelectedCard != null && currentSelectedCard != this)
        {
            currentSelectedCard.animator.SetBool("IsClickBuyBulletBtn", false);
            remove.SetActive(false);
        }
        if (currentSelectedCard == this)
        {
            animator.SetBool("IsClickBuyBulletBtn", false);
            remove.SetActive(false);
            currentSelectedCard = null;
            return;
        }
        animator.SetBool("IsClickBuyBulletBtn", true);
        remove.SetActive(true);
        currentSelectedCard = this;
    }


    // ===========[탄환]=============
    public void OnBuyCard()
    {
        shopCardUI.SetActive(false);
        buyCardUI.SetActive(true);
    }

    public void CardBind(ShopManager.ShopItem item)
    {
        rankText.text = RankLabel(item.ammo.rank);
        rankText2.text = RankLabel(item.ammo.rank);
        suitText.text = SuitLetter(item.ammo.suit);
        suitText2.text = SuitLetter(item.ammo.suit);
        priceText.text = "Ð" + item.price.ToString();
    }

    // ===========[유물 애니메이션]=============
    private void OnRellicClick()
    {
        if (currentSelectedCard != null && currentSelectedCard != this)
        {
            currentSelectedCard.animator.SetBool("IsClickShopRellicBtn", false);
        }
        if(currentSelectedCard == this)
        {
            animator.SetBool("IsClickShopRellicBtn", false);
            currentSelectedCard = null;
            return;
        }
        animator.SetBool("IsClickShopRellicBtn", true);
        currentSelectedCard = this;
    }

    public void OnBuyRelletClick()
    {
        if (currentSelectedCard != null && currentSelectedCard != this)
        {
            currentSelectedCard.animator.SetBool("IsClickBuyRellicBtn", false);
        }
        if (currentSelectedCard == this)
        {
            animator.SetBool("IsClickBuyRellicBtn", false);
            currentSelectedCard = null;
            return;
        }
        animator.SetBool("IsClickBuyRellicBtn", true);
        currentSelectedCard = this;
    }



    // ===========[유물]=============
    public void OnBuyRellic()
    {
        shopRellicUI.SetActive(false);
        buyRellicUI.SetActive(true);
    }
    public void RellicBind(ShopManager.ShopItem item, string decs)
    {
        rellicName.text = item.name;
        rellicName2.text = item.name;
        rellicDesc.text = decs;
        rellicDesc2.text = decs;
        rellicPrice.text = "Ð" + item.price.ToString();
    }

    public void ChangScele()
    {
        buyRelletBtn.transform.localScale = Vector3.one;
    }

    // ===========[유틸]=============

    public void CheckItemType(ShopManager.ShopItem item)
    {
        if(item.type == ShopItemType.Bullet)
        {
            bulletUI.SetActive(true);
            rellicUI.SetActive(false);
        }
        else if(item.type == ShopItemType.SpecialTotem)
        {
            bulletUI.SetActive(false);
            rellicUI.SetActive(true);
        }
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
