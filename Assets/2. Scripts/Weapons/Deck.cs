using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //시작 덱 장수
    //현재 사용안함
    //[SerializeField] private int initialDeckSize = 13;

    //현재 덱
    [SerializeField] 
    //덱 잔량만 유지
    public int Count => GameManager.ItemControl.drawPile.Count;

    private bool isDeck = false;
    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.SelectDeck, BuildInitialDeck);
        if (!isDeck)
        {
            BuildInitialDeck();
            isDeck = true;
        }
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.SelectDeck, BuildInitialDeck);
    }

    //덱 상단에서 탄환 뽑기(부족하면 가진 만큼)
    public List<Ammo> DrawAmmos(int amount)
    {
        var res = new List<Ammo>();
        var draw = GameManager.ItemControl.drawPile;

        // 요청이 0 이하이거나 덱이 없거나 비어 있으면 그대로 종료
        if (amount <= 0 || draw == null || draw.Count == 0) 
            return res;

        // 남은 장 수 만큼만 뽑는다(마지막 1장도 정상 소비)
        int take = Mathf.Min(amount, draw.Count);
        for (int i = 0; i < take; i++)
        {
            int last = draw.Count - 1;
            res.Add(draw[last]);
            draw.RemoveAt(last);
        }

        // 여기서 어떤 '리필'도 하지 않는다. (덱이 0이 되면 그대로 빈 상태 유지)
        return res;
    }

    //외부 UI용
    public List<Ammo> GetDrawSnapshot() => new List<Ammo>(GameManager.ItemControl.drawPile);

    
    public void GetPlayerBullets(List<Ammo> pile)
    {
        for (int i = 0; i < GameManager.ItemControl.drawPile.Count; i++)
        {
            pile.Add(GameManager.ItemControl.drawPile[i]);
        }
    }

    //초기 덱 구성/셔플
    private void BuildInitialDeck()
    {
        GameManager.ItemControl.drawPile.Clear();
        //==============================
        //기본덱
        //숫자 1~13 각 1장, 문양은 랜덤
        //==============================
        //나중에 테스트 끝나면 안에 넣기

        
        if(GameManager.TurnBased.turnSettingValue.IsBasicDeck == true)
        {
            var deck = GameManager.Data.bulletDataGroup.GetBulletData(9005);

            for (int r = 1; r <= deck.max; r++)
            {
                Suit fixedSuit = (Suit)UnityEngine.Random.Range(0, 4);
                GameManager.ItemControl.drawPile.Add(new Ammo { suit = fixedSuit, rank = r });
                
            }
        }
        
            

        //==============================
        //다이아 덱
        //숫자 1~13 각 1장, 문양은 다이아몬드
        //==============================
        if (GameManager.TurnBased.turnSettingValue.IsDiamondDeck == true)
        {
            var deck2 = GameManager.Data.bulletDataGroup.GetBulletData(9003);
            Suit fixedSuit = (Suit)deck2.type;
            for (int r = 1; r <= deck2.max; r++)
            {
                GameManager.ItemControl.drawPile.Add(new Ammo { suit = fixedSuit, rank = r });
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
            for (int r = 1; r <= deck3.max; r++)
            {
                GameManager.ItemControl.drawPile.Add(new Ammo { suit = fixedSuit2, rank = r });

                if (r == 1 || r == 13)
                {
                    GameManager.ItemControl.drawPile.Add(new Ammo { suit = fixedSuit2, rank = r });
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
            for (int r = 1; r <= deck4.max; r++)
            {
                GameManager.ItemControl.drawPile.Add(new Ammo { suit = fixedSuit3, rank = r });
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
            for (int r = 1; r <= deck5.max; r++)
            {
                GameManager.ItemControl.drawPile.Add(new Ammo { suit = fixedSuit4, rank = r });
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
        Shuffle(GameManager.ItemControl.drawPile);
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



