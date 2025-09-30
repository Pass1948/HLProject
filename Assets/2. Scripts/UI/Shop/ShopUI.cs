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

    [SerializeField] private Transform playerBulletRoot;

    // [SerializeField] private Transform powderRoot;
    [SerializeField] private Transform removeRoot;
    [SerializeField] private GameObject cardPrefab; // ShopCardUI 컴포넌트 포함 프리팹
    
    
    [SerializeField] private TextMeshProUGUI rerollCostText;
    [SerializeField] private TextMeshProUGUI healCost;

    private int selectedBulletIndex = -1;
    
    public Button rerollButton;
    public Button healButton;
    public Button removeButton;
    public Button nextStageButton;

    private readonly List<GameObject> spawned = new();

    private void Awake()
    {
        shop = GameManager.Shop;
    }

    private void Start()
    {
    }
    private void OnEnable()
    {
        
        // EventBus 구독
        GameManager.Event.Subscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        GameManager.Event.Subscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Subscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
        GameManager.Event.Subscribe(EventType.ShopPlayerCardsConfim,RebuildPlayerBullets);
        
        healButton.onClick.AddListener(()=> shop.TryHeal());
        rerollButton.onClick.AddListener(()=> shop.TryReroll());
        removeButton.onClick.AddListener(OnRemoveBulletCicked);
        nextStageButton.onClick.AddListener(NextStage);
        if (shop != null) Rebuild(shop.offers);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        GameManager.Event.Unsubscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Unsubscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
        GameManager.Event.Unsubscribe(EventType.ShopPlayerCardsConfim, RebuildPlayerBullets);
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
        // spawned.Clear();
        
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
                default:
                    parent = null; break;
            }
            if(parent == null) continue;
            
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

    private void RebuildPlayerBullets()
    {
        int cost = 1;
        ClearSection(playerBulletRoot);
        Debug.Log($"{playerBulletRoot}");
        var bullets = GameManager.ItemControl.drawPile; 
        Debug.Log($"{bullets}");
        for (int i = 0; i < bullets.Count; i++)
        {
            Debug.Log($"{bullets[i]}");
            var ammo = bullets[i];
            int idx = i;
            var card = GameManager.UI.CreateSlotUI<ShopCardUI>(playerBulletRoot.transform);
            card.Bind(new ShopManager.ShopItem(ShopItemType.Bullet, ammo.ToString(),cost,ammo));
            card.OpenUI();
            card.buyButton.onClick.RemoveAllListeners();
            card.buyButton.onClick.AddListener(() =>
            {
                selectedBulletIndex = idx;
            });
        }
        cost++;
    }

    private void OnRemoveBulletCicked()
    {
        if (selectedBulletIndex >= 0)
        {
            var player = GameManager.Unit.Player.playerHandler;
            if (selectedBulletIndex < player.bullets.Count)
            {
                
                player.bullets.RemoveAt(selectedBulletIndex);
            }
        }
        selectedBulletIndex = -1;

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

    
    private void NextStage()
    {
        // TODO: 여기에 추가해 주시면 됩니당.(JBS)
        int nextStageIndex = GameManager.Shop.stage.GetCurrentStageIndex() + 1;
        GameManager.SaveLoad.nextSceneIndex = nextStageIndex;
        GameManager.SceneLoad.RestartScene();
    }
}