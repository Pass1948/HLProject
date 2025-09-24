using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    // 상점 시스템에 필요한 부분들 정리
    /*
     * 1. 상점에 입장.
     *  - 리롤 2로 초기화, 탄환제거 가능
     *  - 탄환 3~5개, 특수 유물 4개 화약 꾸러미 2회
     *  - (모든 탄환과 유물은 겹치지 않아야 한다.)
     *
     * 2. 탄환 구매
     *  - 가격 = (3 + 화약의 레어도에 따라 추가)
     *  - 달란트 차감 > 탄환집에 추가 > 해당 슬롯 제거
     *
     * 3. 특수 유물 구매
     *  - 레어도에 따라 가격 (5/7/9/11) > 인벤토리에 추가 > 슬롯 제거
     *  - 이미 가지고 있다면 처음에 거름
     *
     * 4. 화약 꾸러미 구매
     *  - 달란트 지불 > 잔여 횟수 감소,
     *  - UI 에서 보유탄환 6 + 화약유물 3 후보 > 1:1 적용?
     *  - 강화 재적용 시 덮어쓰기(기존 강화 제거 후 새 강화 적용)
     *
     * 5. 리롤
     *  - 현재 리롤비 지불 > 리롤비는 +1 씩 증가 > 모든 오퍼 재생성 > 이전 상태로 복구 불가
     *
     * 6. 응급 처치
     *  - 1달란트 지불 > 체력 + 1
     * 
     * 7. 탄환 제거
     *  - 3달란트 지불 > 입장당 1회 소모
     *  - 보유 중인 6개 중 1개 선택 제거(건너뛰기 쌉가능하나 비용/권 회수 없음)
     *
     * 슈발 이게 뭐야.
     * 다시다시 
     * 카드-------------
     * 지금 그럼 해야 하는 방식이 일단, 카드를 셔플을 통해 섞어놓고,
     * 플레이어가 가지고 있는 카드를 빼고, 나머지 중에 최대 5개만큼만 보여준다.
     * ----------------
     *
     * 화약-------------
     * 화약은 화약 자체가 리스트.
     * 화약을 선택했을 때, 랜덤한 카드 6장을 보여줘야 한다.
     * 보여줄 때도 셔플, 그리고 가지고 있는 카드를 가지고 와야한다.
     * 카드를 가지고 올 때 강화된 것도 가지고 온다.
     * 3개의 화약중에 하나를 카드에 삽입 할 수 있다.
     * 이미 강화된 상태라면, 전에 있던 것을 빼고 새로운 것을 넣어준다.
     * -----------------
     * 
     *
     * 
     * 세개가 있는 화약은 
     */
     [Header("참조")]
    public Deck deck;                   // 덱
    public PlayerHandler player;        // 플레이어
    public List<RelicData> relicPool;   // 유물 풀 (JSON 로드된 리스트)
    public List<PowderData> powderPool; // 화약 풀 (JSON 로드된 리스트)

    [Header("상점 상태")]
    public List<ShopItem> offers = new();
    public int rerollCost;
    public bool canRemoveBullet;
    public int powderBundleLeft;

    public float attachPowderChance = 0.5f; // 탄환에 화약이 붙을 확률

    public float weightNormal = 20f;
    public float weightRare = 5f;
    public float weightElite = 3f;
    public float weightLegendary = 1f;

    private void Start()
    {
        EnterShop();
    }


    /// 상점 입장 > 상품 생성
    public void EnterShop()
    {
        rerollCost = 2;
        canRemoveBullet = true;
        powderBundleLeft = 2;
        GenerateOffers();
    }

    /// <summary>상품 구성만 다시 생성(리롤 시 이거만 호출)</summary>
    private void GenerateOffers()
    {
        offers.Clear();

        // 1) 탄환
        GenerateCardOffers(player.bullets);

        // 2) 유물
        GenerateRelicOffers(player.ownedRelics);

        // 3) 화약 꾸러미 (남은 개수만 표시)
        for (int i = 0; i < powderBundleLeft; i++)
            offers.Add(new ShopItem(ShopItemType.PowderBundle, "화약 꾸러미", 4));

        // 4) 응급 처치
        offers.Add(new ShopItem(ShopItemType.Heal, "응급 처치", 1));

        // 5) 탄환 제거 (입장당 1회)
        if (canRemoveBullet)
            offers.Add(new ShopItem(ShopItemType.RemoveBullet, "탄환 제거", 3));

        // 6) 리롤 (현재 비용 표시)
        offers.Add(new ShopItem(ShopItemType.Reroll, "리롤", rerollCost));

        Debug.Log("상점 상품 재구성 완료");
    }

    // ────────────── 탄환 오퍼 ──────────────
    private void GenerateCardOffers(List<Ammo> playerOwned)
    {
        var snapshot = deck.GetDrawSnapshot();
        Shuffle(snapshot);

        int want = GetBulletOfferCount();    // 기본 3~5, 유물로 +2 같은 버프가 있다면 여기서 반영
        int added = 0;

        foreach (var card in snapshot)
        {
            // 보유 중이면 제외(중복 구매 방지)
            bool owned = playerOwned.Exists(p => p.suit == card.suit && p.rank == card.rank);
            if (owned) continue;

            // 카드 복제(스냅샷 원본 변조 방지)
            var offerAmmo = new Ammo { suit = card.suit, rank = card.rank, powder = null };

            // 확률적으로 화약 부착
            if (powderPool != null && powderPool.Count > 0 && Random.value < attachPowderChance)
            {
                var pickedPowder = PickPowderByWeightedRarity();
                offerAmmo.powder = pickedPowder;
            }

            int price = 3;
            if (offerAmmo.powder != null)
                price += GetPowderPrice(offerAmmo.powder.rarity);

            offers.Add(new ShopItem(ShopItemType.Bullet, offerAmmo.ToString(), price, offerAmmo));
            added++;
            if (added >= want) break;
        }
    }
    private int GetBulletOfferCount()
    {
        //TODO: 유물,버프에 의해 슬롯이 증가되면 여기서 계산.
        return Random.Range(3, 6);
    }


    // ────────────── 유물 오퍼 ──────────────
    private void GenerateRelicOffers(List<int> ownedRelics)
    {
        var usedIds = new HashSet<int>();
        var relicCandidates = new List<RelicData>(relicPool);
        Shuffle(relicCandidates);

        foreach (var relic in relicCandidates)
        {
            if (ownedRelics.Contains(relic.id)) continue; // 이미 보유
            if (usedIds.Contains(relic.id)) continue;     // 이번 상점 내 중복

            usedIds.Add(relic.id);

            int price = GetRelicPrice(relic.rarityType);
            offers.Add(new ShopItem(ShopItemType.SpecialTotem, relic.name, price, null, relic));

            if (usedIds.Count >= 4) break; // 최대 4개
        }
    }


    // ────────────── 구매 처리 ──────────────
    public void TryBuy(int index)
    {
        if (index < 0 || index >= offers.Count) return;
        var item = offers[index];

        if (!player.SpendGold(item.price))
            return;

        switch (item.type)
        {
            case ShopItemType.Bullet:
                player.AddBullet(item.ammo);
                RemoveOfferAt(index);
                break;

            case ShopItemType.SpecialTotem:
                if (item.relic != null)
                {
                    player.AddRelic(item.relic.id);
                    RemoveOfferAt(index);
                }
                break;

            case ShopItemType.PowderBundle:
                if (powderBundleLeft > 0)
                {
                    powderBundleLeft--;
                    StartPowderBundle(player.bullets);
                    RemoveOfferAt(index);
                }
                break;

            case ShopItemType.Heal:
                player.Heal(1);
                break;

            case ShopItemType.RemoveBullet:
                if (canRemoveBullet)
                {
                    StartRemoveBullet(player.bullets);
                    canRemoveBullet = false;
                    RemoveOfferAt(index);
                    Debug.Log("탄환 제거 UI 열기");
                }
                break;

            case ShopItemType.Reroll:
                TryReroll();
                break;
        }
    }

    private void RemoveOfferAt(int index)
    {
        if (index >= 0 || index < offers.Count)
            offers.RemoveAt(index);
    }
    private void TryReroll()
    {
        if (!player.SpendGold(rerollCost)) return;

        rerollCost++;
        EnterShop();
    }

    // ────────────── 화약 꾸러미 ──────────────
    private void StartPowderBundle(List<Ammo> playerOwned)
    {
        var candidates = new List<Ammo>(playerOwned);
        Shuffle(candidates);
        
        if (candidates.Count > 6)
            candidates = candidates.GetRange(0, 6);

       
        var powderCandidates = BuildPowderCandidates(3);
        // 이벤트 연결
    }
    // UI에서 선택 완료 시 호출(탄환 1개 + 화약 1개)
    public void ConfirmPowderBundle(Ammo target, PowderData powder)
    {
        if(target == null) return;
        ApplyPowder(target, powder);
    }

    public void ApplyPowder(Ammo target, PowderData powder)
    {
        target.powder = powder;
    }

    private void StartRemoveBullet(List<Ammo> playerOwned)
    {
        var candidates = new List<Ammo>(playerOwned);
        Shuffle(candidates);
        if (candidates.Count > 6)
            candidates = candidates.GetRange(0, 6);
        
        // 이벤트 추가 
    }

    public void ConfirmRemoveBullet(Ammo target)
    {
        if(target == null) return;
        player.RemoveBullet(target);
    }

    // ────────────── 가격 계산 (RarityType 기반) ──────────────
    private int GetPowderPrice(RarityType rarity) => rarity switch
    {
        RarityType.Common    => 0, 
        RarityType.Normal    => 1,
        RarityType.Rare      => 3,
        RarityType.Elite     => 5,
        RarityType.Legendary => 7,
        _ => 0
    };

    private int GetRelicPrice(RarityType rarity) => rarity switch
    {
        RarityType.Common    => 3,
        RarityType.Normal    => 5,
        RarityType.Rare      => 7,
        RarityType.Elite     => 9,
        RarityType.Legendary => 11,
        _ => 0
    };

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private List<PowderData> BuildPowderCandidates(int count)
    {
        var pool =  new List<PowderData>(powderPool);
        Shuffle(pool);
        if(pool.Count <= count) return pool;
        return pool.GetRange(0, count);
    }

    private PowderData PickPowderByWeightedRarity()
    {
        float total = weightNormal + weightRare +  weightElite +  weightLegendary;
        if(total <= 0) return null;
        float roll = Random.Range(0, total);
        float acc = 0f;
        
        RarityType picked = RarityType.Normal;
        acc += weightNormal;
        if (roll <= acc) picked = RarityType.Normal;
        else
        {
            acc += weightRare;
            if (roll <= acc) picked = RarityType.Rare;
            else
            {
                acc += weightRare;
                if (roll <= acc) picked = RarityType.Elite;
                else picked = RarityType.Legendary;
            }
        }
        var same = powderPool.FindAll(p => p.rarity == picked);
        if(same.Count == 0) return powderPool[Random.Range(0, powderPool.Count)];
        return same[Random.Range(0, same.Count)];
    }

    [Serializable]
    public struct ShopItem
    {
        public ShopItemType type;
        public string name;
        public int price;
        public Ammo ammo;
        public RelicData relic; // BaseItem 제거, 바로 RelicData

        public ShopItem(ShopItemType type, string name, int price, Ammo ammo = null, RelicData relic = null)
        {
            this.type = type;
            this.name = name;
            this.price = price;
            this.ammo = ammo;
            this.relic = relic;
        }
    }
}















