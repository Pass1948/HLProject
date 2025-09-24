using System;
using System.Collections.Generic;
using UnityEngine;
using DataTable;
//덱(오토바이): 기본 13장 덱 구성
public class Deck : MonoBehaviour
{
    //시작 덱 장수
    [SerializeField] private int initialDeckSize = 13;
    //시작 풀
    [SerializeField] private List<Ammo> startPool = new();

    //현재 덱
    [SerializeField] private List<Ammo> drawPile = new();
    //덱 잔량만 유지
    public int Count => drawPile.Count;

    private void Awake()
    {
            
            
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        //덱 구성
        BuildInitialDeck();
        //섞기
        Shuffle(drawPile);
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

    //초기 덱 구성/셔플
    private void BuildInitialDeck()
    {
        drawPile.Clear();

        /*
        var deck = GameManager.Data.bulletDataGroup.GetBulletData(9005);

        for(int r = deck.min; r <= deck.max; r++)
        {
            var s = (Suit)UnityEngine.Random.Range(0, 4);
            drawPile.Add(new Ammo { suit = s, rank = r });
        }
        */

        //기존 로직인데 올 랜덤덱을 만들때 쓸수있을것같으니 밑에처럼 남겨놓음
        /*
        if (startPool != null && startPool.Count > 0)
        {
            //중복 유지
            drawPile.AddRange(startPool);
            return;
        }

        // 표준 52장 생성하고 셔플한뒤 initialDeckSize만큼 뽑음
        var library52 = BuildStandard();
        Shuffle(library52);

        int take = Mathf.Min(initialDeckSize, library52.Count);
        for (int i = 0; i < take; i++)
            drawPile.Add(library52[i]);
        */
    }

    //52장 라이브러리
    //나중에 이걸로 랜덤 덱 구성할수도 있으니 남겨놓음
    private List<Ammo> BuildStandard()
    {
        var lib = new List<Ammo>(52);
        foreach (Suit s in Enum.GetValues(typeof(Suit)))
            for (int r = 1; r <= 13; r++)
                lib.Add(new Ammo { suit = s, rank = r });
        return lib;
    }

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



