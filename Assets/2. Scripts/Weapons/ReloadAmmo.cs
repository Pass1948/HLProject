using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadAmmo : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Deck deck;                  // 덱(오토바이)
    [SerializeField] private AttackController magazine;  // 탄창 UI 컨트롤러
    [SerializeField] private bool autoReload = true;     // 시작 시 자동 장전

    [Header("Deck/Discard UI")]
    [SerializeField] private RectTransform deckBg;       // 덱 리스트 부모(DeckBg)
    [SerializeField] private RectTransform discardBg;    // 사용 탄 리스트 부모(DiscardBg)
    [SerializeField] private RectTransform ammoPrefab;   // 표시에 쓸 탄 프리팹(Bullet 프리팹)

    void Start()
    {
        if (autoReload)
        {
            Reload();
        }
        
        else RefreshDeckUI();
    }

    public void Reload()
    {
        if (magazine == null)
        {
            Debug.LogError("Not found magazine");
            return;
        }

        // 1) 탄창 비우고 탄 회수
        List<Ammo> discard = magazine.ClearMagazineAndReturnAmmos();

        // 2) 덱 처리(디스카드 적립 + 필요 시 리셔플 + 드로우)
        if (deck != null)
        {
            if (discard.Count > 0) deck.Discard(discard);
            deck.ReshuffleIfNeeded();

            int need = magazine.Capacity;        // 최대 장전 수
            var draw = deck.DrawAmmos(need);     // 덱에서 뽑기
            magazine.AddBullets(draw);           // 탄창 채우기
        }

        // 3) 패널 갱신
        RefreshDeckUI();
    }

    // 덱/디스카드 패널 다시 그리기
    private void RefreshDeckUI()
    {
        if (ammoPrefab == null) return;

        // 덱 영역
        if (deckBg != null)
        {
            ClearChild(deckBg);
            if (deck != null)
            {
                foreach (var a in deck.GetDrawSnapshot())
                    SpawnAmmo(deckBg, a);
            }
        }

        // 사용 탄 영역
        if (discardBg != null)
        {
            ClearChild(discardBg);
            if (deck != null)
            {
                foreach (var a in deck.GetDiscardSnapshot())
                    SpawnAmmo(discardBg, a);
            }
        }
    }

    //보기 전용으로 버튼 비활성
    private void SpawnAmmo(RectTransform parent, Ammo ammo)
    {
        var item = Instantiate(ammoPrefab, parent, false);
        var view = item.GetComponent<BulletSlotView>();
        if (view)
        {
            view.ammo = ammo;
            if (view.bulletBg) view.bulletBg.color = Color.black;
        }
        var btn = item.GetComponentInChildren<Button>(true);
        if (btn) btn.interactable = false; //클릭불가
    }

    // 자식 전부 제거
    private void ClearChild(RectTransform rt)
    {
        if (rt == null) return;
        for (int i = rt.childCount - 1; i >= 0; i--)
            Destroy(rt.GetChild(i).gameObject);
    }
}

