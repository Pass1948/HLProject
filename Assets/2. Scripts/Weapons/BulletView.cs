using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BulletView : MonoBehaviour
{
    public Image bulletBg;//선택 표시용 배경
    [SerializeField] private TMP_Text label;//D2/C6/H5등등 표기

    public Ammo ammo;

    public void RefreshLabel()
    {
        if (!label)
        {
            return;
        }
        label.text = ammo != null ? SuitLetter(ammo.suit) + ammo.rank : string.Empty;
    }

    public void SetBgColor(Color c)
    {
        if (bulletBg)
        {
            bulletBg.color = c;
        }
    }

    private static string SuitLetter(Suit s)
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

