using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : BaseUI
{
    [Header("참조(인스펙터에서 할당)")]
    [SerializeField] private ShopManager shop;
    [SerializeField] private Transform gridRoot;
    [SerializeField] private GameObject cardPrefab; // ShopCardUI 컴포넌트 포함 프리팹
    [SerializeField] private Text rerollCostText;

    private readonly List<GameObject> spawned = new();

    private void Awake()
    {
        if (!shop) Debug.LogWarning("[ShopUI] ShopManager 참조가 비었습니다. 인스펙터에서 할당하세요.");
        if (!gridRoot) Debug.LogWarning("[ShopUI] gridRoot 참조가 비었습니다.");
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
        for (int i = spawned.Count - 1; i >= 0; i--)
            Destroy(spawned[i]);
        spawned.Clear();

        if (offers == null || gridRoot == null) return;

        // 카드 생성 offers 기준으로
        for (int i = 0; i < offers.Count; i++)
        {
            int idx = i;
            var data = offers[i];

            // 1) UIManager 팩토리 사용
            ShopCardUI card = null;
            if (GameManager.UI != null)
            {
                card = GameManager.UI.CreateSlotUI<ShopCardUI>(gridRoot);
            }
            else
            {
                // 2) 직접 프리팹 인스턴스 (UIManager 미사용 프로젝트 호환)
                var go = Instantiate(cardPrefab, gridRoot);
                card = go.GetComponent<ShopCardUI>();
            }

            card.Bind(data);
            card.buyButton.onClick.RemoveAllListeners();
            card.buyButton.onClick.AddListener(() =>
            {
                shop.TryBuy(idx);
                // TryBuy 안에서 Event가 발행되더라도, 즉시 라벨 보수 갱신
                UpdateRerollLabel();
            });

            spawned.Add(card.gameObject);
        }

        UpdateRerollLabel();
    }

    private void UpdateRerollLabel()
    {
        if (rerollCostText != null && shop != null)
            rerollCostText.text = $"리롤 {shop.rerollCost}";
    }
}