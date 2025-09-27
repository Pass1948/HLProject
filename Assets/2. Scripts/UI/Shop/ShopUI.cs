using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI; 

public class ShopUI : BaseUI
{
    [Header("참조(인스펙터에서 할당)")]
    private ShopManager shop = GameManager.Shop;
    [SerializeField] private Transform bulletRoot;
    [SerializeField] private Transform relicRoot;
    // [SerializeField] private Transform powderRoot;
    [SerializeField] private Transform healthRoot;
    [SerializeField] private Transform removeRoot;
    [SerializeField] private GameObject cardPrefab; // ShopCardUI 컴포넌트 포함 프리팹
    [SerializeField] private TextMeshProUGUI rerollCostText;

    private readonly List<GameObject> spawned = new();

    private void Awake()
    {
        shop = GameManager.Shop;
        if (!shop) Debug.LogWarning("[ShopUI] ShopManager 참조가 비었습니다. 인스펙터에서 할당하세요.");
        if (!bulletRoot) Debug.LogWarning("[ShopUI] gridRoot 참조가 비었습니다.");
        if (!cardPrefab) Debug.LogWarning("[ShopUI] cardPrefab 참조가 비었습니다.");
        
    }

    private void OnEnable()
    {
        // EventBus 구독
        GameManager.Event.Subscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        GameManager.Event.Subscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Subscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);

        if (shop != null) Rebuild(shop.offers);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        GameManager.Event.Unsubscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Unsubscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
    }

    // ===== 이벤트 핸들러 =====
    private void OnOffersChanged(List<ShopManager.ShopItem> offers) => Rebuild(offers);

    private void OnPowderBundlePrompt((List<Ammo> ammos, List<PowderData> powders) payload)
    {
        // 모달 열고 선택 결과 콜백에서 Confirm 호출
        var modal = GameManager.UI.GetUI<PowderBundleModalUI>();
        modal.Open(payload.ammos, payload.powders, (ammoIndex, powderIndex) =>
        {
            if (ammoIndex >= 0 && powderIndex >= 0 &&
                ammoIndex < payload.ammos.Count && powderIndex < payload.powders.Count)
            {
                shop.ConfirmPowderBundle(payload.ammos[ammoIndex], payload.powders[powderIndex]);
            }
            modal.CloseUI();
            Rebuild(shop.offers);
            UpdateRerollLabel();
        });
    }

    private void OnRemoveBulletPrompt(List<Ammo> candidates)
    {
        var modal = GameManager.UI.GetUI<RemoveBulletModalUI>();
        modal.Open(candidates, (index) =>
        {
            if (index >= 0 && index < candidates.Count)
                shop.ConfirmRemoveBullet(candidates[index]);

            modal.CloseUI();
            Rebuild(shop.offers);
            UpdateRerollLabel();
        });
    }

    // ===== UI 빌드 =====
    private void Rebuild(List<ShopManager.ShopItem> offers)
    {
        // 기존 카드 정리
        ClearSection(bulletRoot);
        ClearSection(relicRoot);
        // ClearSection(powderRoot);
        spawned.Clear();
        
        if (offers == null) return;

        // 카드 생성 offers 기준으로
        for (int i = 0; i < offers.Count; i++)
        {
            var data = offers[i];
            int idx = i;
            Transform parent;
            switch (data.type)
            {
                case ShopItemType.Bullet:
                    parent = bulletRoot; break;
                case ShopItemType.SpecialTotem:
                    parent = relicRoot; break;
                // case ShopItemType.PowderBundle:
                //     parent = powderRoot; break;
                default:
                    parent = healthRoot; break;
            }

            ShopCardUI card = null;
            card = GameManager.UI.CreateSlotUI<ShopCardUI>(parent);
            card.Bind(data);
            card.buyButton.onClick.RemoveAllListeners();
            card.buyButton.onClick.AddListener(() =>
            {
                shop.TryBuy(idx);
                UpdateRerollLabel();
            });
            spawned.Add(card.gameObject);
        }
        UpdateRerollLabel();
    }

    private void ClearSection(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }

    private void UpdateRerollLabel()
    {
        if (rerollCostText != null && shop != null)
            rerollCostText.text = $"리롤 {shop.rerollCost}";
    }
}