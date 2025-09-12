using System;
using System.Collections.Generic;
using UnityEngine;

// 덱(오토바이): 12장 덱 구성, 셔플, 드로우, 디스카드, 필요시 리셔플
public class Deck : MonoBehaviour
{
    [SerializeField] private int initialDeckSize = 12;      // 시작 덱 장수(기획: 12)
    [SerializeField] private List<Ammo> startPool = new();  // (선택) 시작 풀: 비어있지 않으면 여기서 최대 N장 샘플링

    [SerializeField] private List<Ammo> drawPile = new();     // 현재 덱(상단=리스트 맨 뒤)
    [SerializeField] private List<Ammo> discardPile = new();  // 사용/버린 탄(사용 탄)

    public int Count => drawPile.Count;// 덱 잔량

    private void Awake()
    {
        if (drawPile.Count == 0)
        {
            BuildInitialDeck(); // 12장 덱 구성
            Shuffle(drawPile);  // 섞기
        }
    }

    // 덱 상단에서 탄환 뽑기(부족하면 가진 만큼)
    public List<Ammo> DrawAmmos(int amount)
    {
        var res = new List<Ammo>();
        if (amount <= 0 || drawPile.Count == 0) return res;

        int take = Mathf.Min(amount, drawPile.Count);
        for (int i = 0; i < take; i++)
        {
            int last = drawPile.Count - 1; // 맨 뒤 = 상단
            res.Add(drawPile[last]);
            drawPile.RemoveAt(last);
        }
        return res;
    }

    // 사용or버린 탄을 버린카드에 넣기
    public void Discard(IEnumerable<Ammo> ammos)
    {
        if (ammos == null) return;
        discardPile.AddRange(ammos);
    }

    // 덱이 바닥나면 버린카드를 덱으로 옮겨 섞기
    public void ReshuffleIfNeeded()
    {
        if (drawPile.Count == 0 && discardPile.Count > 0)
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
        }
    }

    //외부 UI용
    public List<Ammo> GetDrawSnapshot() => new List<Ammo>(drawPile);
    public List<Ammo> GetDiscardSnapshot() => new List<Ammo>(discardPile);

    //초기 덱 구성/셔플
    private void BuildInitialDeck()
    {
        drawPile.Clear();
        discardPile.Clear();

        
        if (startPool != null && startPool.Count > 0)
        {
            var poolCopy = new List<Ammo>(startPool);
            Shuffle(poolCopy);
            UniqueTake(poolCopy, drawPile, initialDeckSize);
            return;
        }

        // 기본: 52종 라이브러리에서 12장 가져옴
        var library52 = BuildStandard52Library();
        Shuffle(library52);
        UniqueTake(library52, drawPile, initialDeckSize);
    }

    private List<Ammo> BuildStandard52Library()
    {
        var lib = new List<Ammo>(52);
        foreach (Suit s in Enum.GetValues(typeof(Suit)))
            foreach (Rank r in Enum.GetValues(typeof(Rank)))
                lib.Add(new Ammo { suit = s, rank = r });
        return lib;
    }

    // src에서 Id 중복 없이 최대 count개를 dst로 이동
    private void UniqueTake(List<Ammo> src, List<Ammo> dst, int count)
    {
        var seen = new HashSet<string>();
        for (int i = 0; i < src.Count && dst.Count < count; i++)
        {
            var a = src[i];
            if (a == null) continue;
            if (seen.Add(a.Id))
                dst.Add(a);
        }
    }

    // 피셔-예이츠 셔플방식
    private void Shuffle(List<Ammo> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}



