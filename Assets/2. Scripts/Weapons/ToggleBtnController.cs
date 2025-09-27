using UnityEngine;

public class ToggleBtnController : MonoBehaviour
{
    [SerializeField] private RectTransform deckui;
    [SerializeField] private RectTransform discardui;
    [SerializeField] private RectTransform bikeControllUI;
    [SerializeField] private RectTransform RelicListUI;

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
        bikeControllUI.gameObject.SetActive(!bikeControllUI.gameObject.activeSelf);
    }

    public void ToggleRelicList()
    {
        if(!RelicListUI)
        {
            return;
        }
        RelicListUI.gameObject.SetActive(!RelicListUI.gameObject.activeSelf);
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
}
