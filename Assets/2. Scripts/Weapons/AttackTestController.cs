using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackTestController : MonoBehaviour
{
    [SerializeField] private RectTransform bullet;

    private Color bgNormal = new Color(0.6f, 1f, 0.6f, 1f);
    private Color bgSel = new Color(1f, 0.9f, 0.5f, 1f);

    private TargetTest selectedTarget;
    private Button selectedAmmoBtn;
    

    //타겟지정
    public void SelectTarget(TargetTest target)
    {
        selectedTarget = target;
    }

    //탄환버튼 OnClick
    public void SelectAmmo(Button btn)
    {
        selectedAmmoBtn = btn;
        bullet = btn.transform.parent as RectTransform;
    }

    public void Fire()
    {
        if(selectedAmmoBtn == null)
        {
            return;
        }

        //슬롯(bullet) 전체
        GameObject shootbullet = bullet != null ? bullet.gameObject : selectedAmmoBtn.gameObject; 

        RectTransform layoutRoot = (bullet != null) ? bullet.parent as RectTransform : null;

        Debug.Log("Fire");
        selectedAmmoBtn = null;
        bullet = null;

        Destroy(shootbullet); //발사된 탄환 제거

        if(layoutRoot != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        }
    }

}
