using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    // EnterShop: 상점 상태 초기화 + 오퍼 생성
    //
    // GenerateOffers: 탄환/유물/화약 꾸러미/응급처치/탄환 제거/리롤 → offers 채움
    //
    // TryBuy: 아이템 종류에 따라 구매 처리
    //
    // StartPowderBundle / StartRemoveBullet: 모달 UI 이벤트 발행
    //
    // UI는 GameManager.Event를 구독해서 오퍼가 갱신되면 카드 재생성, 모달 열기 등을 한다.
     [Header("참조")]
    public Deck deck;                    // 전체 탄환 덱
    public PlayerHandler player;         // 플레이어
    private List<RelicData> relicPool;   // 모든 유물 풀 
    public List<PowderData> powderPool;  // 모든 화약 풀 
    private List<Ammo> bundleAmmoCache;  // 
    private List<PowderData> bundlePowderCache; 
    private List<Ammo> removeCandidates;

    [Header("상점 상태")]
    public List<ShopItem> offers = new(); // 상점에 등장한 아이템 목록
    public int rerollCost;                // 현재 리롤 비용
    public int healCost;                  // 현재 힐 비용
    public bool canRemoveBullet;          // 탄환 제거 사용 가능 여부
    public int powderBundleLeft;          // 화역 꾸러미 남은 개수

    public Stage stage;
    private void Start()
    {
        // 아직 바로 실행 중입니다.
        if (relicPool == null || relicPool.Count == 0)
            relicPool = DataTable.RelicData.GetList();
        
        if(powderPool == null)
            powderPool = new List<PowderData>();
    }
    public void ShopInit(Stage stage)
    {
        this.stage = stage;
        deck = GameManager.Resource.Create<Deck>(Path.UI + "Deck");
        canRemoveBullet = true;
        powderBundleLeft = 2;
        GenerateOffers();
        
    }
    
    //상품 구성만 다시 생성(리롤 시 이거만 호출)
    private void GenerateOffers()
    {
        offers.Clear();
        player = GameManager.Unit.Player.playerHandler;
        // 1) 탄환
        GenerateCardOffers(player.bullets);
        // 2) 유물
        GenerateRelicOffers(player.ownedRelics, GameManager.ItemControl.buyItems);
        

        // 탄환 제거 (입장당 1회)
        if (canRemoveBullet)
            offers.Add(new ShopItem(ShopItemType.RemoveBullet, "탄환 제거", 3));
        
        GameManager.Event.Publish(EventType.ShopOffersChanged, offers);
    }
    
    // 탄환 오퍼 
    private void GenerateCardOffers(List<Ammo> playerOwned)
    {
        var snapshot = deck.GetDrawSnapshot();

        Shuffle(snapshot);

        int want = GetBulletOfferCount();
        int added = 0;

        foreach (var card in snapshot)
        {
            // 보유 중이면 제외(중복 구매 방지)
            // List<T>.Exists(Predicate<T>) = 리스트 안에 조건을 만족하는 원소가 하나라도 있으면 true반환
            // bool owned = playerOwned.Exists(p => p.suit == card.suit && p.rank == card.rank);
            // if (owned) continue;

            // 새로운 탄환 후보 생성
            var offerAmmo = new Ammo { suit = card.suit, rank = card.rank, powder = null};
            
            // 가격 = 기본3 + (화약 레어도 추가 비용)
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
        int baseCount = 5;
        int bonusCount = 0;
        return baseCount + bonusCount;
    }


    //  유물 오퍼 
    private void GenerateRelicOffers(List<int> ownedRelics, List<ItemModel> it)
    {
        var usedIds = new HashSet<int>();
        var relicCandidates = GameManager.ItemControl.RelicWeightSampling(GameManager.ItemControl.relicItems.Count);
        Shuffle(relicCandidates);

        foreach (var relic in relicCandidates)
        {
            if (ownedRelics.Contains(relic.id)) continue; // 이미 보유한 유물
            if (it.Contains(relic)) continue;   // 구매한 가방리스트에 저장
            if (usedIds.Contains(relic.id)) continue;     // 이번 상점 내 중복 유물

            usedIds.Add(relic.id);

            int price = GetRelicPrice(relic.rarity);
            offers.Add(new ShopItem(ShopItemType.SpecialTotem, relic.name, price, null, relic));

            if (usedIds.Count >= 4) break; // 최대 4개
        }
    }


    // 때껄룩 
    public void TryBuy(int index)
    {
        if (index < 0 || index >= offers.Count) return; //돈없으면 끄지라
        var item = offers[index];

        if (!player.SpendGold(item.price))
            return;

        bool changed = false;

        switch (item.type)
        {
            case ShopItemType.Bullet:
                GameManager.ItemControl.drawPile.Add(item.ammo);
                RemoveOfferAt(index);
                GameManager.Event.Publish(EventType.ShopPlayerCardsConfim);
                GameManager.Sound.PlayShopSfx();
                changed =  true;
                break;

            case ShopItemType.SpecialTotem:
                if (item.relic != null)
                {
                    GameManager.ItemControl.CreateRelicObject(item.relic.id, item.relic);
                    RemoveOfferAt(index);
                    GameManager.Sound.PlayShopSfx();
                    changed = true;
                }
                break;

            case ShopItemType.PowderBundle:
                if (powderBundleLeft > 0)
                {
                    powderBundleLeft--;
                    StartPowderBundle(player.bullets); // 모달 UI 열기 (파우더 번들 사기)TODO: 지금은 아직 미정
                    RemoveOfferAt(index);
                    GameManager.Sound.PlayShopSfx();
                    changed = true;
                }
                break;

            case ShopItemType.Heal:
                player.Heal(1);
                break;

            case ShopItemType.RemoveBullet:
                if (canRemoveBullet)
                {
                    RemoveOfferAt(index);
                    GameManager.Sound.PlayShopSfx();
                    changed = true;
                }
                break;
        }
        if(changed)
            GameManager.Event.Publish(EventType.ShopOffersChanged, offers);
    }

    private void RemoveOfferAt(int index)
    {
        if (index < 0 && index >= offers.Count) return;
        offers.RemoveAt(index);
    }
    
    // 돈내고 오퍼 전체 재생성
    public void TryReroll()
    {
        if (!player.SpendGold(rerollCost)) return;
        player.SpendGold(rerollCost);
        rerollCost++;
        GenerateOffers();
        GameManager.Sound.PlayShopSfx();
    }
    public void TryHeal()
    {
        if(!player.SpendGold(healCost)) return;
        player.SpendGold(healCost);
        player.Heal(healCost/2);
        healCost++;
        GameManager.Sound.PlayShopSfx();
    }

    // 화약 꾸러미 * 사용중지
    private void StartPowderBundle(List<Ammo> playerOwned)
    {
        var candidates = new List<Ammo>(playerOwned);
        Shuffle(candidates);
        
        if (candidates.Count > 6) candidates = candidates.GetRange(0, 6);
        
        var powderCandidates = BuildPowderCandidates(3);

        bundleAmmoCache = candidates;
        bundlePowderCache = powderCandidates;
        // 이벤트 연결 UI에 신호 + 후보 전달 
        GameManager.Event.Publish(EventType.ShopPowderBundlePrompt,(bundleAmmoCache, bundlePowderCache));
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

    //UI에서 탄환 제거 모달 열기
    private void StartRemoveBullet(List<Ammo> playerOwned)
    {
        var candidates = new List<Ammo>(playerOwned);
        Shuffle(candidates);
        if (candidates.Count > 6)
            candidates = candidates.GetRange(0, 6);
        
        removeCandidates = candidates;
        // 이벤트 추가 UI에 제거 후보 신호 + 후보 전달
        GameManager.Event.Publish(EventType.ShopRemoveBulletPrompt,removeCandidates);
    }
    // 카드 삭제
    public void ConfirmRemoveBullet(Ammo target)
    {
        if(target == null) return;
            GameManager.ItemControl.drawPile.Remove(target);
    }

    // 가격 계산 (RarityType 기반)
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

    // 화약 유물 레어도 계산
    // private PowderData PickPowderByWeightedRarity()
    // {
    //     float total = weightNormal + weightRare +  weightElite +  weightLegendary;
    //     if(total <= 0) return null;
    //     float roll = Random.Range(0, total);
    //     float acc = 0f;
    //     
    //     RarityType picked = RarityType.Normal;
    //     acc += weightNormal;
    //     if (roll <= acc) picked = RarityType.Normal;
    //     else
    //     {
    //         acc += weightRare;
    //         if (roll <= acc) picked = RarityType.Rare;
    //         else
    //         {
    //             acc += weightElite;
    //             if (roll <= acc) picked = RarityType.Elite;
    //             else picked = RarityType.Legendary;
    //         }
    //     }
    //     var same = powderPool.FindAll(p => p.rarity == picked);
    //     if(same.Count == 0) return powderPool[Random.Range(0, powderPool.Count)];
    //     return same[Random.Range(0, same.Count)];
    // }

    [Serializable]
    public struct ShopItem
    {
        public ShopItemType type;
        public string name;
        public int price;
        public Ammo ammo;
        public ItemModel relic; // BaseItem 제거, 바로 RelicData

        public ShopItem(ShopItemType type, string name, int price, Ammo ammo = null, ItemModel relic = null)
        {
            this.type = type;
            this.name = name;
            this.price = price;
            this.ammo = ammo;
            this.relic = relic;
        }
    }
}















