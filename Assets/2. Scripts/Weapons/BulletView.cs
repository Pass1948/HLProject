using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BulletView : MonoBehaviour
{
    public Image bulletBg;//선택 표시용 배경
    [SerializeField] private TMP_Text suitText;//D2/C6/H5등등 표기
    [SerializeField] private TMP_Text numText;

    public Ammo ammo;

    public void RefreshLabel()
    {
        if (!suitText && !numText)
        {
            return;
        }
        suitText.text = SuitLetter(ammo.suit);
        numText.text = RankLabel(ammo.rank);
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
            case Suit.Spade: return "♠";
            case Suit.Heart: return "♥";
            case Suit.Diamond: return "♦";
            case Suit.Club: return "♣";
            default: return "?";
        }
    }

    private static string RankLabel(int rank)
    {
        switch (rank)
        {
            case 1: return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return rank.ToString();
        }
    }
}

