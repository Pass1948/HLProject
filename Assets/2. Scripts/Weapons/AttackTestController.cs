using UnityEngine;
using UnityEngine.UI;

public class AttackTestController : MonoBehaviour
{
    //2025 09 11
    [SerializeField] private RectTransform bullet;

    private Color bgNormal = new Color(0f, 0f, 0f, 1f); //검정색
    private Color bgSel = new Color(1f, 0f, 0f, 1f); //붉은색
    private Button selectedAmmoBtn;

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

        //제거 대상 결정
        //bullet(슬롯)이 있다면 슬롯을 전체 삭제하게함
        GameObject shootbullet = bullet != null ? bullet.gameObject : selectedAmmoBtn.gameObject; 

        //레이아웃 갱신 대상
        RectTransform layoutRoot = (bullet != null) ? bullet.parent as RectTransform : null;

        Debug.Log("Fire");

        //내부 상태 초기화
        selectedAmmoBtn = null;
        bullet = null;

        Destroy(shootbullet); //발사된 탄환 제거

        //레이아웃 즉시 갱신시키기
        if(layoutRoot != null)
        {
            //씬에 있는 모든 Canvas의 대기중인 레이아웃을 즉시 갱신처리
            Canvas.ForceUpdateCanvases(); 
            //전달된 RectTransform 서브트리(대부분 부모) 레이아웃을 즉시 재계산시킴
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
            
        }
    }

}
