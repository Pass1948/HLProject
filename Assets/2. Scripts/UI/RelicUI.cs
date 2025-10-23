using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Selectable

[DisallowMultipleComponent]
public class RelicUI : MonoBehaviour
{
    //보유 유물 리스트쪽
    [SerializeField] private RectTransform content;   // 카드가 붙을 부모(그리드/컨텐츠)

    //info창
    [SerializeField] private RectTransform infoPanel;
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private TMP_Text infoDecs;
    [SerializeField] private TMP_Text infoRare;

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
        var list = GameManager.ItemControl.buyItems;

        cards.Clear();
        hovered = null;
        HideInfo();

        // 활성 구간 채우기
        for (int i = 0; i < list.Count; i++)
        {
            var card = GameManager.UI.CreateSlotUI<ShopCardUI>(content);

            if (!card.gameObject.activeSelf) card.gameObject.SetActive(true);
            var item = new ShopManager.ShopItem(
                ShopItemType.SpecialTotem,
                 list[i]?.name ?? $"Relic {list[i]?.id}",
                0, null, list[i]
            );
            card.CheckItemType(item);
            card.RellicBind(item, item.relic.description);
            card.OnBuyRellic();
            card.ChangScele();
            var rt = card.transform as RectTransform;
            if (rt) cards.Add((rt, list[i]));
        }

        // 여분 비활성
        for (int i = list.Count; i < pool.Count; i++)
        {
            var go = pool[i].gameObject;
            if (go.activeSelf) go.SetActive(false);
        }

        if (cards.Count == 0) HideInfo();
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
        if (infoDecs) infoDecs.text = m.description;
        if (infoRare) infoRare.text = (m.rarity).ToString();
        infoPanel.gameObject.SetActive(true);
    }

    private void HideInfo()
    {
        if (infoPanel && infoPanel.gameObject.activeSelf)
            infoPanel.gameObject.SetActive(false);
    }
}

