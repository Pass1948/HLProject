using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadAmmo : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Deck deck;// 덱(오토바이)
    [SerializeField] private AttackController magazine;// 탄창 UI
    [SerializeField] private bool autoReload = true;// 시작 시 자동 장전

    [Header("Deck/Discard UI")]
    [SerializeField] private RectTransform deckBg;// 덱 리스트
    [SerializeField] private RectTransform discardBg;// 사용 탄 리스트
    [SerializeField] private RectTransform ammoPrefab;// 표시에 쓸 Bullet 프리팹

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

        // 탄창 비우고 탄 회수
        List<Ammo> discard = magazine.ClearMagazineAndReturnAmmos();

        // 덱 처리
        if (deck != null)
        {
            // 회수된 탄을 디스카드로
            if (discard.Count > 0) deck.Discard(discard);

            //덱이 비어 있으면 한 번 섞기
            //안쓰면 루프 한번 헛돌고 정상작동됨
            deck.Reshuffle();

            int need = magazine.Capacity; // 최대 장전 수
            var drawn = new List<Ammo>();

            // 필요한 수량을 다 채울 때까지 반복해서 드로우
            while (drawn.Count < need)
            {
                var batch = deck.DrawAmmos(need - drawn.Count);

                if (batch.Count == 0)
                {
                    // 덱이 바닥났으니 디스카드에서 덱으로 옮겨 섞기 시도
                    deck.Reshuffle();

                    // 여전히 덱이 비어 있으면(디스카드도 소진), 더 이상 뽑을 수 없음
                    if (deck.Count == 0) break;

                    // 섞였으니 다시 드로우 시도
                    continue;
                }

                drawn.AddRange(batch);
            }

            // 실제 탄창 채우기
            if (drawn.Count > 0)
                magazine.AddBullets(drawn);
        }

        // UI 갱신
        RefreshDeckUI();
    }

    //덱, 디스카드 새로고침
    private void RefreshDeckUI()
    {
        if (ammoPrefab == null) return;

        //덱
        if (deckBg != null)
        {
            ClearChild(deckBg);
            if (deck != null)
            {
                foreach (var a in deck.GetDrawSnapshot())
                    SpawnAmmo(deckBg, a);
            }
        }

        //디스카드
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

        //클릭불가
        if (btn)
        {
            btn.interactable = false;
        }
    }

    // 자식 전부 제거
    private void ClearChild(RectTransform rt)
    {
        if (rt == null) return;
        for (int i = rt.childCount - 1; i >= 0; i--)
            Destroy(rt.GetChild(i).gameObject);
    }
}

