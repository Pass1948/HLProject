using UnityEngine;
using DG.Tweening;

public class ToggleBtnController : MonoBehaviour
{
    [SerializeField] private RectTransform deckui;
    [SerializeField] private RectTransform discardui;
    [SerializeField] private RectTransform bikeControllUI;
    [SerializeField] private RectTransform relicListUI;
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform guideUI;
    //덱 토글
    public void ToggleDeck()
    {
        if (!deckui)
        {
            return;
        }

        bool IsDeckOpenAfter = !deckui.gameObject.activeSelf;
        deckui.gameObject.SetActive(IsDeckOpenAfter);

        //버린 탄환 켜있으면 닫아주기
        if(IsDeckOpenAfter && discardui)
        {
            discardui.gameObject.SetActive(false);
        }
    }

    //버린 탄환 토글
    public void ToggleDiscard()
    {
        if (!discardui)
        {
            return;
        }
        bool IsDiscardOpenAfter = !discardui.gameObject.activeSelf;
        discardui.gameObject.SetActive(IsDiscardOpenAfter);

        //탄환집 켜있으면 닫아주기
        if(IsDiscardOpenAfter && deckui)
        {
            deckui.gameObject.SetActive(false);
        }
    }

    //오토바이 조작버튼 토글
    public void ToggleBikeControll()
    {
        if(!bikeControllUI)
        {
            return;
        }
        if (GameManager.Mouse.isShowRange)
        {
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
            bikeControllUI.gameObject.SetActive(!bikeControllUI.gameObject.activeSelf);
        }
    }

    public void ToggleRelicList()
    {
        if (!relicListUI)
        {
            return;
        }

        bool IsRelicOpenAfter = !relicListUI.gameObject.activeSelf;
        relicListUI.gameObject.SetActive(IsRelicOpenAfter);

        // 열릴 때만 그리기
        if (IsRelicOpenAfter)
        {
            relicListUI.GetComponent<RelicUI>()?.Render();
        }
    }


    public void ToggleKick()
    {
        if (GameManager.Mouse.IsKicking && GameManager.Mouse.isShowRange)
        {
            GameManager.Map.attackRange.ClearAttackType();
            GameManager.Mouse.IsKicking = false;
        }
        else
        {
            //현재 꺼져 있으면 켠다
            GameManager.Map.attackRange.SetAttackRangeForKick();
            GameManager.Mouse.IsKicking = true;
        }
    }


    public void ToggleMove()
    {
        if (GameManager.Mouse.isShowMoveRange)
        {
            GameManager.Mouse.OnClickPlayer(GameManager.Map.GetPlayer3Position());
        }
    }

    //가이드 UI패널을 토글함
    public void ToggleGuide()
    {
        if (!guideUI) return;
        guideUI.gameObject.SetActive(!guideUI.gameObject.activeSelf);

    }
}
