using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [SerializeField] private RectTransform slotContainer;
    [SerializeField] private RectTransform bulletSlotPrefab;
    [SerializeField] private RectTransform discardBg;

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
        if(btn == null)
        {
            return;
        }


        //이미 사용된 슬롯 클릭 방지
        if (!btn.interactable)
        {
            return;
        }

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

        //배경 이미지 가져오고 저장
        //BulletSlotView가 붙은 부모를 찾아 RectTransform을 정확히 집는다
        var slotView = btn.GetComponentInParent<BulletSlotView>();
        var slotRoot = slotView ? (RectTransform)slotView.transform : null;
        var bg = slotView ? slotView.bulletBg : null;

        if(slotRoot == null)
        {
            return;
        }

        //디스카드쪽 선택 금지
        if(discardBg != null && slotRoot.parent == discardBg)
        {
            return;
        }

        selectedAmmoBtn = btn;
        bullet = slotRoot;
        selectBulletBg = bg;

        if (selectBulletBg)
        {
            selectBulletBg.color = bgSel;
        }
    }

    public void Fire()
    {
        if (!selectedAmmoBtn || !bullet)
        {
            return;
        }

        //발사되고 배경색 초기화
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }

        var view = bullet.GetComponent<BulletSlotView>();

        //디스카드랑 덱에서는 버튼 비활성
        var btn = bullet.GetComponentInChildren<Button>(true);
        if (btn)
        {
            btn.interactable = false;
        }

        if (view?.bulletBg != null)
        {
            view.bulletBg.color = Color.black;
        }

        //발사된 탄환 Discard
        if (discardBg != null)
        {
            bullet.SetParent(discardBg, false);
            bullet.localScale = Vector3.one;
            
            bullet.GetComponent<AmmoLabelView>()?.RefreshLabel();
            Refresh(discardBg);
        }
        else
        {
            Debug.LogError("Not Found discardBg");
        }

        //상태 정리
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;

        Refresh(slotContainer);
        Debug.Log("Fire");
    }

    //탄창 비우고 탄 리스트 반환
    public List<Ammo> ClearMagazine()
    {
        var result = new List<Ammo>();

        //선택된 탄환 색 되돌리기
        if (selectBulletBg)
        {
            selectBulletBg.color = bgNormal;
        }
        
        for (int i = slotContainer.childCount - 1; i >= 0; i--)
        {
            var slot = (RectTransform)slotContainer.GetChild(i);
            var view = slot.GetComponent<BulletSlotView>();
            if (view && view.ammo != null) result.Add(view.ammo);

            //클릭 불가 처리
            var btn = slot.GetComponentInChildren<Button>(true);
            if (btn) btn.interactable = false;

            if (view?.bulletBg != null) view.bulletBg.color = Color.black;
            slot.GetComponent<AmmoLabelView>()?.RefreshLabel();

            //DiscardBg로 부모 변경
            if (discardBg != null)
            {
                slot.SetParent(discardBg, false);
                slot.localScale = Vector3.one;
            }
            else
            {
                Debug.LogError("discardBg is null");
                Destroy(slot.gameObject);
            }
        }

        // 선택 상태 리셋
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;

        // 레이아웃 갱신
        Refresh(slotContainer);
        if (discardBg)
        {
            Refresh(discardBg);
        }

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
        Refresh(slotContainer);
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
        
        slot.GetComponent<AmmoLabelView>()?.RefreshLabel();

        var btn = slot.GetComponentInChildren<Button>(true);
        if(btn)
        {
            btn.onClick.AddListener(() => SelectAmmo(btn));
        }
    }


    //레이아웃 즉시 갱신
    private static void Refresh(RectTransform root)
    {
        if (root == null)
        {
            return;
        }
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }
    

}
