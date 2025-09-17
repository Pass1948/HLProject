using UnityEngine;

public class DeckDiscardOnOff : MonoBehaviour
{
    [SerializeField] private RectTransform deckui;
    [SerializeField] private RectTransform discardui;

    public void ToggleDeck()
    {
        if (!deckui)
        {
            return;
        }
        deckui.gameObject.SetActive(!deckui.gameObject.activeSelf);
    }

    public void ToggleDiscard()
    {
        if (!discardui)
        {
            return;
        }
        discardui.gameObject.SetActive(!discardui.gameObject.activeSelf);
    }
}
