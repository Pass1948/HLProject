using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadAmmo : MonoBehaviour
{
    [SerializeField] private Deck deck;// 덱(오토바이)
    [SerializeField] private AttackController magazine;// 탄창 UI
    [SerializeField] private bool autoReload = true;// 시작 시 자동 장전

    [SerializeField] private RectTransform deckBg;// 덱 리스트
    [SerializeField] private RectTransform discardBg;// 사용 탄 리스트
    [SerializeField] private RectTransform ammoPrefab;// 표시에 쓸 Bullet 프리팹

    void Start()
    {
        if (autoReload)
        {
            Reload();
        }
        else
        {
            RefreshDeckUI();
        }   
    }

    public void Reload()
    {
        if (magazine == null)
        {
            Debug.LogError("Not found magazine");
            return;
        }

        //탄창 비우기
        magazine.ClearMagazine();

        //탄환 뽑기
        if(deck != null)
        {
            int need = magazine.Capacity;
            var draw = deck.DrawAmmos(need);
            magazine.AddBullets(draw);
        }
        
        // UI 갱신
        RefreshDeckUI();
    }

    //덱, 디스카드 새로고침
    private void RefreshDeckUI()
    {
        if (ammoPrefab == null || deckBg == null)
        {
            return;
        }

        ClearChild(deckBg);

        //덱
        if (deck != null)
        {
            foreach(var a in deck.GetDrawSnapshot())
            {
                SpawnAmmo(deckBg, a);
            }
        }

        /*
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
        */
    }

    //보기 전용으로 버튼 비활성
    private void SpawnAmmo(RectTransform parent, Ammo ammo)
    {
        var item = Instantiate(ammoPrefab, parent, false);
        var view = item.GetComponent<BulletSlotView>();
        if (view)
        {
            view.ammo = ammo;
        }

        item.GetComponent<AmmoLabelView>()?.RefreshLabel();

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

