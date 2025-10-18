using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Selectable

[DisallowMultipleComponent]
public class RelicUI : MonoBehaviour
{
    //보유 유물 리스트쪽
    [SerializeField] private RectTransform content;   // 카드가 붙을 부모(그리드/컨텐츠)
    [SerializeField] private ShopCardUI cardPrefab;   // 상점 카드 프리팹 (그대로 재사용)

    //info창
    [SerializeField] private RectTransform infoPanel;
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private TMP_Text infoRare;

    private ItemControlManger icm;
    private Canvas rootCanvas;
    private Camera uiCam;

    //오브젝트 풀
    private readonly List<ShopCardUI> pool = new();
    //스냅샷 리스트
    private readonly List<(RectTransform rt, ItemModel model)> cards = new();
    private (RectTransform rt, ItemModel model)? hovered = null;

    private Vector3 lastMouse; // 마우스 움직임 체크

    private static readonly List<ItemModel> EmptyList = new(0);

    private void Awake()
    {
        //한번 캐싱해서 부담 줄이기
        icm = GameManager.ItemControl != null ? GameManager.ItemControl
                                              : FindFirstObjectByType<ItemControlManger>();
        rootCanvas = GetComponentInParent<Canvas>();
        uiCam = rootCanvas ? rootCanvas.worldCamera : null;
        HideInfo();
    }

    private void OnEnable()
    {
        Render();
    }
    private void OnDisable()
    {
        HideInfo(); hovered = null;
    }

    // ===== 렌더링 =====
    public void Render()
    {
        if (!content || !cardPrefab) return;

        if (icm == null)
            icm = GameManager.ItemControl ?? FindFirstObjectByType<ItemControlManger>();

        var list = icm?.buyItems ?? EmptyList;

        EnsurePool(list.Count);

        cards.Clear();
        hovered = null;
        HideInfo();

        // 활성 구간 채우기
        for (int i = 0; i < list.Count; i++)
        {
            var card = pool[i];
            var relic = list[i];

            if (!card.gameObject.activeSelf) card.gameObject.SetActive(true);

            var item = new ShopManager.ShopItem(
                ShopItemType.SpecialTotem,
                relic?.name ?? $"Relic {relic?.id}",
                0, null, relic
            );
            card.Bind(item);

            // 가격/제목 숨김
            var priceTf = card.transform.Find("Price");
            if (priceTf) priceTf.gameObject.SetActive(false);

            var titleTf = card.transform.Find("Title");
            if (titleTf) titleTf.gameObject.SetActive(false);

            // 구매 비활성
            if (card.buyButton)
            {
                card.buyButton.interactable = false;
                card.buyButton.transition = Selectable.Transition.None;
                var nav = card.buyButton.navigation; nav.mode = Navigation.Mode.None; card.buyButton.navigation = nav;
            }

            var rt = card.transform as RectTransform;
            if (rt) cards.Add((rt, relic));
        }

        // 여분 비활성
        for (int i = list.Count; i < pool.Count; i++)
        {
            var go = pool[i].gameObject;
            if (go.activeSelf) go.SetActive(false);
        }

        if (cards.Count == 0) HideInfo();
    }

    private void EnsurePool(int need)
    {
        while (pool.Count < need)
        {
            var inst = Instantiate(cardPrefab, content);
            pool.Add(inst);
        }
    }

    // ===== Hover 검사 (마우스 움직였을 때만) =====
    private void Update()
    {
        if (!isActiveAndEnabled) return;
        if (!infoPanel || cards.Count == 0) return;

        var m = Input.mousePosition;
        if (m == lastMouse) return; // 마우스 정지 중이면 스킵
        lastMouse = m;

        (RectTransform rt, ItemModel model)? hit = null;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var (rt, model) = cards[i];
            if (!rt) continue;
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, m, uiCam))
            {
                hit = (rt, model);
                break;
            }
        }

        if (hit?.rt != hovered?.rt)
        {
            hovered = hit;
            if (hovered != null) ShowInfo(hovered.Value.model);
            else HideInfo();
        }
    }

    // ===== Info 표시 =====
    private void ShowInfo(ItemModel m)
    {
        if (m == null || !infoPanel) { HideInfo(); return; }
        if (infoName) infoName.text = m.name;
        if (infoRare) infoRare.text = (m.rarity).ToString();
        infoPanel.gameObject.SetActive(true);
    }

    private void HideInfo()
    {
        if (infoPanel && infoPanel.gameObject.activeSelf)
            infoPanel.gameObject.SetActive(false);
    }
}

