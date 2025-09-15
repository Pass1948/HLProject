using UnityEngine;
using TMPro;

public class AmmoLabelView : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private BulletSlotView slot;

    void Awake()
    {
        slot = GetComponent<BulletSlotView>();
    }
    void Start()
    {
        
    }

    public void RefreshLabel()
    {
        if(!label)
        {
            return;
        }

        var a = slot ? slot.ammo : null;
        label.text = FormatShort(a);
    }

    private static string FormatShort(Ammo a)
    {
        if (a == null)
        {
            return "";
        }
        return SultLetter(a.suit) + a.rank;
    }

    private static string SultLetter(Suit s)
    {
        switch (s)
        {
            case Suit.Spade: return "S";
            case Suit.Heart: return "H";
            case Suit.Diamond: return "D";
            case Suit.Club: return "C";
            default: return "?";
        }
    }
}
