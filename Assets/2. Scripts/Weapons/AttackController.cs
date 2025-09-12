using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [SerializeField] private RectTransform bullet;
    private Button selectedAmmoBtn;
    private Image selectBulletBg;

    private Color bgNormal = new Color(0f, 0f, 0f, 1f);
    private Color bgSel = new Color(1f, 0f, 0f, 1f);

    [SerializeField] private RectTransform slotContainer;
    [SerializeField] private RectTransform bulletSlotPrefab;
    [SerializeField] private int AmmoCount = 6;

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
        if(bullet != null)
        {
            var bgTr = bullet.Find("BulletBg");
            if(bgTr != null)
            {
                selectBulletBg = bgTr.GetComponent<Image>();
            }
        }

        //선택하면 붉은색
        if(selectBulletBg !=null)
        {
            selectBulletBg.color = bgSel;
        }
    }

    public void Fire()
    {
        if(selectedAmmoBtn == null)
        {
            return;
        }

        //삭제될 탄환 선언
        GameObject shootbullet = bullet.gameObject;
        RectTransform layoutRoot = slotContainer;

        Debug.Log("Fire");

        //상태정리
        selectedAmmoBtn = null;
        selectBulletBg = null;
        bullet = null;

        Destroy(shootbullet); //발사된 탄환 제거

        //레이아웃 즉시 갱신
        if(layoutRoot != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        }
    }

}
