using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //시작 덱 장수
    //현재 사용안함
    //[SerializeField] private int initialDeckSize = 13;
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)
    //TODO: 바꿘거 아니라메요ㅜㅜ 카드가 상점에 안나와서 10시간을 버렸어요ㅠㅠ(JBS)

    //현재 덱
    [SerializeField] private List<Ammo> drawPile = new();
    //덱 잔량만 유지
    public int Count => drawPile.Count;

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.SelectDeck, BuildInitialDeck);
        BuildInitialDeck();
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.SelectDeck, BuildInitialDeck);
    }

    //덱 상단에서 탄환 뽑기(부족하면 가진 만큼)
    public List<Ammo> DrawAmmos(int amount)
    {
        var res = new List<Ammo>();
        if (amount <= 0 || drawPile.Count == 0) return res;

        int take = Mathf.Min(amount, drawPile.Count);
        for (int i = 0; i < take; i++)
        {
            int last = drawPile.Count - 1;
            res.Add(drawPile[last]);
            drawPile.RemoveAt(last);
        }
        return res;
    }

    //외부 UI용
    public List<Ammo> GetDrawSnapshot() => new List<Ammo>(drawPile);

    /*
     * 어디에 넣어주지
     * 플레이어에 넣어주면 되나
     * 
     */
    public void GetPlayerBullets(List<Ammo> pile)
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            pile.Add(drawPile[i]);
        }
    }

    //초기 덱 구성/셔플
    private void BuildInitialDeck()
    {
        drawPile.Clear();
        //==============================
        //기본덱
        //숫자 1~13 각 1장, 문양은 랜덤
        //==============================

            var deck = GameManager.Data.bulletDataGroup.GetBulletData(9005);

            for (int r = deck.min; r <= deck.max; r++)
            {
                var s = (Suit)UnityEngine.Random.Range(0, 4);
                drawPile.Add(new Ammo { suit = s, rank = r });
            }

        //==============================
        //다이아 덱
        //숫자 1~13 각 1장, 문양은 다이아몬드
        //==============================
        if (GameManager.TurnBased.turnSettingValue.IsDiamondDeck == true)
        {
            var deck2 = GameManager.Data.bulletDataGroup.GetBulletData(9003);
            Suit fixedSuit = (Suit)deck2.type;
            for (int r = deck2.min; r <= deck2.max; r++)
            {
                drawPile.Add(new Ammo { suit = fixedSuit, rank = r });
            }
        }

        //==============================
        //하트 덱
        //숫자 1이랑 13 2장,2~12 각 1장, 문양은 하트
        //==============================
        if (GameManager.TurnBased.turnSettingValue.IsHeartDeck == true)
        {
            var deck3 = GameManager.Data.bulletDataGroup.GetBulletData(9002);
            Suit fixedSuit2 = (Suit)deck3.type;
            for (int r = deck3.min; r <= deck3.max; r++)
            {
                drawPile.Add(new Ammo { suit = fixedSuit2, rank = r });

                if (r == 1 || r == 13)
                {
                    drawPile.Add(new Ammo { suit = fixedSuit2, rank = r });
                }
            }
        }

        //==============================
        //스페이드 덱
        //숫자 1~13 각 1장, 문양은 스페이드
        //==============================
        if (GameManager.TurnBased.turnSettingValue.IsSpadeDeck == true)
        {
            var deck4 = GameManager.Data.bulletDataGroup.GetBulletData(9001);
            Suit fixedSuit3 = (Suit)deck4.type;
            for (int r = deck4.min; r <= deck4.max; r++)
            {
                drawPile.Add(new Ammo { suit = fixedSuit3, rank = r });
            }
        }

        //==============================
        //클로버 덱
        //숫자 1~13 각 1장, 문양은 클로버
        //==============================
        if (GameManager.TurnBased.turnSettingValue.IsClubDeck == true)
        {
            var deck5 = GameManager.Data.bulletDataGroup.GetBulletData(9004);
            Suit fixedSuit4 = (Suit)deck5.type;
            for (int r = deck5.min; r <= deck5.max; r++)
            {
                drawPile.Add(new Ammo { suit = fixedSuit4, rank = r });
            }
        }

        //기존 로직인데 올 랜덤덱을 만들때 쓸수있을것같으니 밑에처럼 남겨놓음
        /*
        // 표준 52장 생성하고 셔플한뒤 initialDeckSize만큼 뽑음
        var library52 = BuildStandard();
        Shuffle(library52);

        int take = Mathf.Min(initialDeckSize, library52.Count);
        for (int i = 0; i < take; i++)
            drawPile.Add(library52[i]);
        */

        //덱만들고 섞기
        Shuffle(drawPile);
    }

    //52장 라이브러리
    //나중에 이걸로 올랜덤 덱 구성할수도 있으니 남겨놓음
    /*
    private List<Ammo> BuildStandard()
    {
        var lib = new List<Ammo>(52);
        foreach (Suit s in Enum.GetValues(typeof(Suit)))
            for (int r = 1; r <= 13; r++)
                lib.Add(new Ammo { suit = s, rank = r });
        return lib;
    }
    */

    //피셔-예이츠 셔플방식
    //랜덤보다 메모리가 더 효율적임
    private void Shuffle(List<Ammo> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}



