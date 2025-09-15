using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [SerializeField] private RectTransform slotContainer;
    [SerializeField] private RectTransform bulletSlotPrefab;
    [SerializeField] private RectTransform discardBg;
    [SerializeField] private Deck deck;

    private RectTransform bullet;
    private Button selectedAmmoBtn;
    private Image selectBulletBg;

    private Color bgNormal = new Color(0f, 0f, 0f, 1f);
    private Color bgSel = new Color(1f, 0f, 0f, 1f);


    [SerializeField] private int AmmoCount = 6;
    public int Capacity => AmmoCount;

    //탄환버튼 OnClick
    public void SelectAmmo(Button btn)
    {
        //같은 탄환을 다시 클릭시 선택해제
        if (selectedAmmoBtn == btn)
        {
            if (selectBulletBg) selectBulletBg.color = bgNormal;
            selectedAmmoBtn = null;
            selectBulletBg = null;
            bullet = null;
            return;
        }

        //이전 선택 복구
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }

        selectedAmmoBtn = btn;
        bullet = btn.transform.parent as RectTransform;

        //배경 이미지 가져오고 저장
        selectBulletBg = null;
        if (bullet)
        {
            var view = bullet.GetComponent<BulletSlotView>();
            if (view && view.bulletBg) selectBulletBg = view.bulletBg;
        }

        if (selectBulletBg)
        {
            selectBulletBg.color = bgSel;
        }
    }

    public void Fire()
    {
        if (!selectedAmmoBtn || !bullet) return;

        //발사되고 배경색 초기화
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }
        
        //파괴 전 탄 데이터
        var view = bullet.GetComponent<BulletSlotView>();
        Ammo ammoFired = view?.ammo;

        //디스카드에 누적
        if (deck != null && ammoFired != null)
        {
            deck.Discard(new[] { ammoFired });
        }
            
        //디스카드랑 덱에서는 버튼 비활성
        var btn = bullet.GetComponentInChildren<Button>(true);
        if (btn) btn.interactable = false;

        if (view?.bulletBg != null)
            view.bulletBg.color = Color.black;

        //발사된 탄환 Discard
        if (discardBg != null)
        {
            bullet.SetParent(discardBg, false);
            bullet.localScale = Vector3.one;
            Rebuild(discardBg);
        }
        else
        {
            Debug.LogError("[AttackController] discardBg가 비어 있어 슬롯을 옮길 수 없습니다.");
        }

        //상태 정리
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;

        Rebuild(slotContainer);
        Debug.Log("Fire");
    }

    //Reload용 공개
    public List<Ammo> ClearMagazineAndReturnAmmos()
    {
        var result = new List<Ammo>();

        for (int i = slotContainer.childCount - 1; i >= 0; i--)
        {
            var slot = (RectTransform)slotContainer.GetChild(i);
            var view = slot.GetComponent<BulletSlotView>();
            if (view && view.ammo != null) result.Add(view.ammo);
            Destroy(slot.gameObject);
        }

        selectedAmmoBtn = null; // 선택 상태 리셋
        selectBulletBg = null;
        bullet = null;

        Rebuild(slotContainer);
        return result;
    }

    // 덱에서 받은 Ammo 리스트로 채우기
    public void AddBullets(List<Ammo> ammos)
    {
        if (ammos != null)
        {
            foreach (var a in ammos)
            {
               SpawnOne(a);
            }
        }
        Rebuild(slotContainer);
    }

    //버튼 연결
    private void SpawnOne(Ammo ammo)
    {
        if (slotContainer == null || bulletSlotPrefab == null)
        {
            Debug.LogError("Not Found slotContainer or bulletSlotPrefab");
            return;
        }

        RectTransform slot = Instantiate(bulletSlotPrefab, slotContainer, false);
        slot.localScale = Vector3.one;

        var view = slot.GetComponent<BulletSlotView>();
        if (view)
        {
            view.ammo = ammo;//어떤 탄인지
            if (view.bulletBg)
            {
                // 기본색 세팅
                view.bulletBg.color = bgNormal;
            }
        }
        var btn = slot.GetComponentInChildren<Button>(true);
        // 런타임 리스너 연결
        if (btn) btn.onClick.AddListener(() => SelectAmmo(btn)); 
    }


    //레이아웃 즉시 갱신
    private static void Rebuild(RectTransform root)
    {
        if (root == null)
        {
            return;
        }
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }
    

}
