using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class ShopUI : BaseUI
{
    [Header("참조(인스펙터에서 할당)")]
    private ShopManager shop;

    private PlayerHandler player;
    [SerializeField] private Transform bulletRoot;
    [SerializeField] private Transform relicRoot;
    [SerializeField] private Transform playerBulletRoot;
    [SerializeField] private Image hpBar;
    
    [SerializeField] private TextMeshProUGUI rerollCostText;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI healCost;

    private int selectedBulletIndex = -1;
    private int maxHp;
    private int currentHp;
    
    private bool isOn =  false;
    
    public Button rerollButton;
    public Button healButton;
    public Button removeButton;
    public Button nextStageButton;
    public Button settingsButton;
    
    public GameObject settingsPanel;
    
    private readonly List<GameObject> spawned = new();

    private void Awake()
    {
        shop = GameManager.Shop;
        player = GameManager.Unit.Player.playerHandler;

    }
    private void OnEnable()
    {
        // EventBus 구독
        GameManager.Event.Subscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        // GameManager.Event.Subscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Subscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
        GameManager.Event.Subscribe(EventType.ShopPlayerCardsConfim,RebuildPlayerBullets); // 소지 카드 체크
        GameManager.Event.Subscribe(EventType.ShopPlayerCardsConfim,PlayerHpCheck);       // 체력 체크
        
        healButton.onClick.AddListener(PlayerHeal);
        rerollButton.onClick.AddListener(()=> shop.TryReroll());
        removeButton.onClick.AddListener(OnRemoveBulletClicked);
        nextStageButton.onClick.AddListener(NextStage);
        settingsButton.onClick.AddListener(OnSettingButton);
        if (shop != null) Rebuild(shop.offers);
        
        RebuildPlayerBullets();
    }


    private void OnDisable()
    {
        GameManager.Event.Unsubscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        // GameManager.Event.Unsubscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Unsubscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
        GameManager.Event.Unsubscribe(EventType.ShopPlayerCardsConfim, RebuildPlayerBullets);
        GameManager.Event.Unsubscribe(EventType.ShopPlayerCardsConfim, PlayerHpCheck);
    }

    // 이벤트 핸들러
    private void OnOffersChanged(List<ShopManager.ShopItem> offers) => Rebuild(offers);

    // 건파우더 모달
    // private void OnPowderBundlePrompt((List<Ammo> ammos, List<PowderData> powders) payload)
    // {
    //     // 모달 열고 선택 결과 콜백에서 Confirm 호출
    //     var modal = GameManager.UI.GetUI<PowderBundleModalUI>();
    //     modal.Open(payload.ammos, payload.powders, (ammoIndex, powderIndex) =>
    //     {
    //         if (ammoIndex >= 0 && powderIndex >= 0 &&
    //             ammoIndex < payload.ammos.Count && powderIndex < payload.powders.Count)
    //         {
    //             shop.ConfirmPowderBundle(payload.ammos[ammoIndex], payload.powders[powderIndex]);
    //         }
    //         modal.CloseUI();
    //         Rebuild(shop.offers);
    //         UpdateRerollLabel();
    //     });
    // }

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
        removeButton.interactable = false;
        PlayerMoneyText();
        
        if (offers == null) return;

        // 카드 생성 offers 기준으로
        for (int i = 0; i < offers.Count; i++)
        {
            var data = offers[i];
            int idx = i;

            Transform parent = data.type switch
            {
                ShopItemType.Bullet => bulletRoot,
                ShopItemType.SpecialTotem => relicRoot,
                _ => null
            };
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
        var bullets = GameManager.ItemControl.drawPile; 
        for (int i = 0; i < bullets.Count; i++)
        {
            var ammo = bullets[i];
            int idx = i;
            var card = GameManager.UI.CreateSlotUI<ShopCardUI>(playerBulletRoot);
            card.Bind(new ShopManager.ShopItem(ShopItemType.Bullet, ammo.ToString(),cost,ammo));
            card.OpenUI();
            card.buyButton.onClick.RemoveAllListeners();
            card.buyButton.onClick.AddListener(() =>
            {
                selectedBulletIndex = idx;
                removeButton.interactable = true;
            });
        }
        cost++;
    }

    private void PlayerHeal()
    {
        shop.TryHeal();
        PlayerHpCheck();
    }
    
    private void PlayerHpCheck()
    {
        currentHp = GameManager.Unit.Player.playerModel.health;
        maxHp = GameManager.Unit.Player.playerModel.maxHealth;
        float fill = (float)currentHp / (float)maxHp;
        hpBar.fillAmount = fill;
    }

    private void OnRemoveBulletClicked()
    {
        if (selectedBulletIndex >= 0)
        {
            var drowPile = GameManager.ItemControl.drawPile;
            if (selectedBulletIndex < drowPile.Count)
            {
                drowPile.RemoveAt(selectedBulletIndex);
                GameManager.Event.Publish(EventType.ShopPlayerCardsConfim);
                RebuildPlayerBullets();
            }
        }
        selectedBulletIndex = -1;
        removeButton.interactable = false; // 선택 초기화 시 비활성
    }
    private void ClearSection(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }

    // 리롤
    private void UpdateRerollLabel()
    {
        if (rerollCostText != null && shop != null)
            rerollCostText.text = $"리롤 {shop.rerollCost}";
    }

    // 플레이어 돈
    private void PlayerMoneyText()
    {
        if (playerMoneyText != null)
            playerMoneyText.text = $"{player.playerMonney}";
    }
    // 세팅 버튼
    private void OnSettingButton()
    {
        isOn = !isOn;
        settingsPanel.SetActive(isOn);
    }

    private void NextStage()
    {
        // TODO: 여기에 추가해 주시면 됩니당.(JBS)
        int nextStageIndex = GameManager.Shop.stage.GetCurrentStageIndex() + 1;

        GameManager.Unit.CurrentStatReset();
        GameManager.SaveLoad.nextSceneIndex += nextStageIndex;
        GameManager.Stage.stageId++;
        GameManager.SceneLoad.RestartScene();
    }
}