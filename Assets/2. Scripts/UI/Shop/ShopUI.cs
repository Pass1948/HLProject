using DG.Tweening;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : BaseUI
{

    [Header("참조(인스펙터에서 할당)")] private ShopManager shop;

    private PlayerHandler player;
    [SerializeField] private Transform bulletRoot;
    [SerializeField] private Transform relicRoot;
    [SerializeField] private Transform playerBulletRoot;
    [SerializeField] private Transform playerRellicRoot;
    [SerializeField] private Image hpBar;

    [SerializeField] private TextMeshProUGUI rerollCostText;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI healCost;
    [SerializeField] private TextMeshProUGUI hpText;

    private int selectedBulletIndex = -1;
    private int maxHp;
    private int currentHp;

    private bool isOn = false;

    public Button rerollButton;
    public Button healButton;
    public Button removeButton;
    public Button nextStageButton;
    public Button settingsButton;

    [SerializeField] private Button rellicInvenBtn;
    [SerializeField] private GameObject PlayerBulletsInfoUI;
    [SerializeField] private GameObject settingUI;


    private readonly List<GameObject> spawned = new();

    private void Awake()
    {
        shop = GameManager.Shop;
        player = GameManager.Unit.Player.playerHandler;

    }

    private void OnEnable()
    {

        OnAnalyticsEvent_open(GameManager.SaveLoad.nextSceneIndex);
        GameManager.Sound.PlayBGM(GameManager.Resource.Create<AudioClip>(Path.Sound + "Buy some cards!"));
        healButton.onClick.AddListener(PlayerHeal);
        rerollButton.onClick.AddListener(OnReroll);
        removeButton.onClick.AddListener(OnRemoveBulletClicked);
        nextStageButton.onClick.AddListener(NextStage);
        settingsButton.onClick.AddListener(OnSettingButton);
        rellicInvenBtn.onClick.AddListener(OnOpenInven);
        currentHp = GameManager.Unit.Player.playerModel.currentHealth;
        maxHp = GameManager.Unit.Player.playerModel.maxHealth;
        hpText.text = $"{currentHp}/{maxHp}";
        float fill = (float)currentHp / (float)maxHp;
        hpBar.fillAmount = fill;
        shop.healCost = 4;
        shop.rerollCost = 2;
        // EventBus 구독
        GameManager.Event.Subscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        // GameManager.Event.Subscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Subscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
        GameManager.Event.Subscribe(EventType.ShopPlayerCardsConfim, RebuildPlayerBullets); // 소지 카드 체크
        GameManager.Event.Subscribe(EventType.ShopPlayerCardsConfim, PlayerHpCheck); // 체력 체크

        if (shop != null) Rebuild(shop.offers);

        RebuildPlayerBullets();
    }


    private void OnDisable()
    {
        healButton.onClick.RemoveListener(PlayerHeal);
        rerollButton.onClick.RemoveListener(OnReroll);
        removeButton.onClick.RemoveListener(OnRemoveBulletClicked);
        nextStageButton.onClick.RemoveListener(NextStage);
        settingsButton.onClick.RemoveListener(OnSettingButton);
        rellicInvenBtn.onClick.RemoveListener(OnOpenInven);
        GameManager.Event.Unsubscribe<List<ShopManager.ShopItem>>(EventType.ShopOffersChanged, OnOffersChanged);
        // GameManager.Event.Unsubscribe<(List<Ammo>, List<PowderData>)>(EventType.ShopPowderBundlePrompt, OnPowderBundlePrompt);
        GameManager.Event.Unsubscribe<List<Ammo>>(EventType.ShopRemoveBulletPrompt, OnRemoveBulletPrompt);
        GameManager.Event.Unsubscribe(EventType.ShopPlayerCardsConfim, RebuildPlayerBullets);
        GameManager.Event.Unsubscribe(EventType.ShopPlayerCardsConfim, PlayerHpCheck);
        
    }
    private void OnAnalyticsEvent_open(int v)
    {
        // TODO : shop_open_stage
        CustomEvent customEvent = new CustomEvent("shop_open_stage")
        {
            { "stageValue", v}
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
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


    //===== 인벤토리 관련 로직 =====
    private void OnOpenInven()
    {
        bool willOpen = !PlayerBulletsInfoUI.activeSelf;
        PlayerBulletsInfoUI.SetActive(willOpen);

        string spriteName = willOpen
            ? "Chest_Luckybox_Bronze_Open"
            : "Chest_Luckybox_Bronze";

        rellicInvenBtn.image.sprite = GameManager.Resource.Load<Sprite>(Path.UI + "Image/" + spriteName);
    }

    //===== 상점에 총알과 아이템 배치 관련 로직 =====
    private void OnRemoveBulletPrompt(List<Ammo> candidates)
    {
        var modal = GameManager.UI.GetUI<RemoveBulletModalUI>();
        modal.Open(candidates, (index) =>
        {
        /*    if (index >= 0 && index < candidates.Count)
                shop.ConfirmRemoveBullet(candidates[index]);*/

            modal.CloseUI();
            Rebuild(shop.offers);
            UpdateRerollLabel();
            UpdateHPLabel();
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
            Transform parent = data.type
                switch
            {
                ShopItemType.Bullet => bulletRoot,
                ShopItemType.SpecialTotem => relicRoot,
                _ => null
            };
            if (data.type == ShopItemType.Bullet)
            {
                if (parent == null) continue;
                ShopCardUI card = null;
                card = GameManager.UI.CreateSlotUI<ShopCardUI>(parent);
                card.CheckItemType(data);
                card.CardBind(data);
                card.bulletBtn.onClick.RemoveAllListeners();
                card.bulletBtn.onClick.AddListener(() =>
                {
                    shop.TryBuy(idx);
                    UpdateRerollLabel();
                });
                spawned.Add(card.gameObject);
            }

            if (data.type == ShopItemType.SpecialTotem)
            {
                if (parent == null) continue;
                ShopCardUI card = null;
                card = GameManager.UI.CreateSlotUI<ShopCardUI>(parent);
                card.CheckItemType(data);
                card.RellicBind(data, data.relic.description);
                card.rellicBtn.onClick.RemoveAllListeners();
                card.rellicBtn.onClick.AddListener(() =>
                {
                    shop.TryBuy(idx);
                    RebuildPlayerRellics();
                    UpdateRerollLabel();
                });
                spawned.Add(card.gameObject);
            }

        }

        UpdateRerollLabel();
        UpdateHPLabel();
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
            card.CardBind(new ShopManager.ShopItem(ShopItemType.Bullet, ammo.ToString(), cost, ammo));
            card.OnBuyCard();
            card.OpenUI();
            card.buyBulletBtn.onClick.RemoveAllListeners();
            card.buyBulletBtn.onClick.AddListener(() =>
            {
                card.OnPlayerCard(removeButton.gameObject);
                selectedBulletIndex = idx;
                removeButton.interactable = true;
            });
        }
        cost++;
    }
    private void RebuildPlayerRellics()
    {
        ClearSection(playerRellicRoot);
        var bullets = GameManager.ItemControl.buyItems;
        for (int i = 0; i < bullets.Count; i++)
        {
            var rellic = bullets[i];
            int idx = i;
            var card = GameManager.UI.CreateSlotUI<ShopCardUI>(playerRellicRoot);
            var rellicitem = new ShopManager.ShopItem(ShopItemType.SpecialTotem, rellic.name, 1, null, rellic);
            card.RellicBind(rellicitem, rellic.description);
            card.CheckItemType(rellicitem);
            card.OnBuyRellic();
            card.OpenUI();
            PlayerMoneyText();
            PlayerHpCheck();
        }
    }

    private void PlayerHeal()
    {
        UpdateHPLabel();
        var seq = DOTween.Sequence();
        seq.Append(healButton.transform.DOScale(2.7f, 0.2f));
        seq.Append(healButton.transform.DOScale(2.4f, 0.2f));
        shop.TryHeal();
        healCost.text = "Ð" + shop.healCost.ToString();
        PlayerMoneyText();
        PlayerHpCheck();
    }

    private void PlayerHpCheck()
    {
        currentHp = GameManager.Unit.Player.playerModel.currentHealth;
        maxHp = GameManager.Unit.Player.playerModel.maxHealth;
        hpText.text = $"{currentHp}/{maxHp}";
        float fill = (float)currentHp / (float)maxHp;
        hpBar.fillAmount = fill;

    }

    private void OnReroll()
    {
        var seq = DOTween.Sequence();
        seq.Append(rerollButton.transform.DOScale(2.7f, 0.2f));
        seq.Append(rerollButton.transform.DOScale(2.4f, 0.2f));
        shop.TryReroll();
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
        removeButton.gameObject.SetActive(false);
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
            rerollCostText.text = "Ð" + shop.rerollCost.ToString();
    }

    // HP cost
    private void UpdateHPLabel()
    {
        if (healCost != null && shop != null)
            healCost.text = "Ð" + shop.healCost.ToString();
    }

    // 플레이어 돈
    private void PlayerMoneyText()
    {
        if (playerMoneyText != null)
            playerMoneyText.text = "Ð" + GameManager.Unit.Player.playerModel.monney.ToString();
    }

    // 세팅 버튼
    private void OnSettingButton()
    {
        isOn = !isOn;
        settingUI.transform.DOLocalMove(new Vector2(0, 0), 0.8f);
    }

    private void NextStage()
    {
        OnAnalyticsEvent_close(GameManager.SaveLoad.nextSceneIndex);

        // TODO: 여기에 추가해 주시면 됩니당.(JBS)
        int nextStageIndex = GameManager.Shop.stage.GetCurrentStageIndex() + 1;
        GameManager.Unit.CurrentStatReset();
        GameManager.SaveLoad.nextSceneIndex += nextStageIndex;
        GameManager.Stage.stageId++;
        GameManager.SceneLoad.RestartScene();
    }
    private void OnAnalyticsEvent_close(int v)
    {
        // TODO : shop_close_stage
        CustomEvent customEvent = new CustomEvent("shop_close_stage")
        {
            { "stageValue", v}
        };
        AnalyticsService.Instance.RecordEvent(customEvent);
    }

}