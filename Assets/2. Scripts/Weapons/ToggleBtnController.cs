using UnityEngine;

public class ToggleBtnController : MonoBehaviour
{
    [SerializeField] private RectTransform deckui;
    [SerializeField] private RectTransform discardui;
    [SerializeField] private RectTransform bikeControllUI;
    [SerializeField] private RectTransform relicListUI;
    [SerializeField] private RectTransform panel;

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
        panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        bikeControllUI.gameObject.SetActive(!bikeControllUI.gameObject.activeSelf);
    }

    public void ToggleRelicList()
    {
        if(!relicListUI)
        {
            return;
        }
        relicListUI.gameObject.SetActive(!relicListUI.gameObject.activeSelf);
    }

    public void ToggleKick()
    {
        if (GameManager.Mouse.IsKicking)
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
        if (GameManager.Mouse.isMoving && GameManager.Mouse.isShowRange)
        {
            GameManager.Mouse.ShowPlayerRange(GameManager.Map.GetPlayer3Position());
            GameManager.Mouse.isMoving = false;
        }
        else
        {
            //현재 꺼져 있으면 켠다
            GameManager.Mouse.HidePlayerRange();
            GameManager.Mouse.isMoving = true;
        }
    }
}
