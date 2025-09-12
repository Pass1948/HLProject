using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [SerializeField] private RectTransform slotContainer;
    [SerializeField] private RectTransform bulletSlotPrefab;

    private RectTransform bullet;
    private Button selectedAmmoBtn;
    private Image selectBulletBg;

    private Color bgNormal = new Color(0f, 0f, 0f, 1f);
    private Color bgSel = new Color(1f, 0f, 0f, 1f);


    [SerializeField] private int AmmoCount = 6;
    public int Capacity => AmmoCount;

    void Start()
    {
        SpawnInitialBullet();
    }

    //시작시 탄환을 AmmoCount만큼 생성
    private void SpawnInitialBullet()
    {
        if(slotContainer == null || bulletSlotPrefab == null)
        {
            Debug.LogError("Not Found bullet");
            return;
        }

        for (int i = 0; i < AmmoCount; i++)
        {
            RectTransform slot = Instantiate(bulletSlotPrefab, slotContainer);
            slot.name = $"Bullet({i + 1})";

            //Bg 기본색을 세팅
            var bgTr = slot.Find("BulletBg");
            if(bgTr && bgTr.TryGetComponent(out Image bgImg))
            {
                bgImg.color = bgNormal;
            }

            // 버튼 클릭 연결
            if(slot.GetComponentInChildren<Button>(true) is Button btn)
            {
                Button captured = btn;
                captured.onClick.AddListener(() => SelectAmmo(captured));
            }
        }

        //레이아웃을 즉시 갱신시켜 UI에 적용시킴
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
    }

    //탄환버튼 OnClick
    public void SelectAmmo(Button btn)
    {
        //선택 해제시 배경색 복구
        if(selectBulletBg != null)
        {
            selectBulletBg.color = bgNormal;
        }

        //현재 선택 저장
        selectedAmmoBtn = btn;
        bullet = btn.transform.parent as RectTransform;

        //탄환 배경찾기
        selectBulletBg = null;
        if(bullet)
        {
            var view = bullet.GetComponent<BulletSlotView>();
            if(selectBulletBg)
            {
                selectBulletBg.color = bgSel;
            }
        }

        
    }

    public void Fire()
    {
        if(!selectedAmmoBtn || !bullet)
        {
            return;
        }

        Destroy(bullet.gameObject); //발사된 탄환 제거
        
        //상태정리
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
            foreach (var a in ammos)
                SpawnOne(a);

        Rebuild(slotContainer);
    }

    // 슬롯 하나 생성 + 데이터/버튼 연결
    private void SpawnOne(Ammo ammo)
    {
        if (slotContainer == null || bulletSlotPrefab == null)
        {
            Debug.LogError("[AttackController] slotContainer 또는 bulletSlotPrefab이 비어있습니다.");
            return;
        }

        RectTransform slot = Instantiate(bulletSlotPrefab, slotContainer, false);
        slot.localScale = Vector3.one;

        var view = slot.GetComponent<BulletSlotView>();
        if (view)
        {
            view.ammo = ammo;                          // 어떤 탄인지(없으면 null)
            if (view.bulletBg) view.bulletBg.color = bgNormal; // 기본색 세팅
            // (원하면) ammo → 아이콘/색 매핑으로 총알 비주얼 지정
        }

        var btn = slot.GetComponentInChildren<Button>(true);
        if (btn) btn.onClick.AddListener(() => SelectAmmo(btn)); // 런타임 리스너 연결
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
