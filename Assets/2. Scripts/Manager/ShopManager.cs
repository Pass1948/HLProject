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
    public Deck deck; // 플레이어 덱 참조
    public List<PowderData> powders; // 화약
    public List<RelicData> relics; // 유물 풀
    
    
    public Rare rare;
    public List<ShopItem> offers = new(); // 상점 카드 오퍼
    public int rerollCost = 1; // 현재 리롤 비용
    public bool canRemoveBullet; // 탄환 제거 가능 여부
    public int powerBundleLeft; // 화약 꾸러미 남은 횟수
    
    private int playerGold; //달란티

    private void Start()
    {
        playerGold = GameManager.Unit.Player.playerHandler.playerMonney;
    }
//플레이어 모델 ㅡ게임메니저,데이터 메니저

    public void ShopStart(List<string> playerOwnedRelics)
    {
        offers.Clear();
        rerollCost = 1;
        canRemoveBullet = false;
        powerBundleLeft = 2;
        
        
        // 카드 오퍼 
        
        // 특수 유물 오퍼
        
        // 화약꾸러미(2개 슬롯)
        
        // 리롤 버튼
        
        // 응급처치
        
        // 탄환제거AA
        
        
        
    }
    // 카드 오퍼 생성(플레이어 소지 제외, 최대 5개)
    private void GenerateCardOffers(List<Ammo> playerOwned)
    {
        // 스
        var snapShot = deck.GetDrawSnapshot();
        Shuffle(snapShot);

        int added = 0;
        foreach (var card in snapShot)
        {
            // Exists는 리스트 안에 조건을 만족하는 요소가 하나라도 있으면 true 없으면 false 반환.
            // p는 playerOwned 안에 있는 각 탄환(Ammo)뜻 한다.
            // 비교 기준은 무늬가 같은가?, 숫자가 같은가? = 둘 다 같은 카드가 있으면 true
            bool owned = playerOwned.Exists(p => p.suit == card.suit && p.rank == card.rank);
            if(owned) continue;

            int price = 3;
            if (card.powder != null)
            {
                
            }
        }
    }

    private void GenerrateRelicOffers(RelicData relicManager)
    {
        var pool = new List<RelicData>(relics);
        Shuffle(pool);

        int added = 0;
        foreach (var relic in pool)
        {
        }
    }
    
    
    
    
    
    public void TryBuy(int index)
    {
        if(index < 0 || index >= offers.Count) return;
        var item = offers[index];

        if (playerGold < item.price)
        {
            // 돈 부족 하다는 유아이 추가ㅇ
            return;
        }
        
        playerGold -= item.price;

        switch (item.type)
        {
            case ShopItemType.Bullet :
                Debug.Log("자 총알을 샀네요");
                // 이벤트 추가
                break;
            case ShopItemType.SpecialTotem:
                Debug.Log("자 유물을 샀네요"); 
                break;
            case ShopItemType.PowderBundle :
                Debug.Log("자 화약을 샀네요");  
                break;
            case ShopItemType.Heal : 
                Debug.Log("자 지 회복할 아이템을 샀네요"); // 사면 바로 힐되면 됨
                break;
            case ShopItemType.RemoveBullet:
                Debug.Log("자 총알 지우기를 샀네요"); // 총알 
                break;
            case ShopItemType.Reroll:
                
                break;
        }
    }

    private void TryReroll()
    {
        if (playerGold < rerollCost)
        {
            // 돈 부족 유아이 추가
            return;
        }
        playerGold -= rerollCost;
        rerollCost++;
        ShopStart(new List<string>());
    }

    private void StartPowderBundle(List<Ammo> playerOwned)
    {
        // 복사본 새 리스트
        var candidates = new List<Ammo>(playerOwned);
        Shuffle(candidates);
        
        if(candidates.Count > 6)
            candidates = candidates.GetRange(0,6);
        var pool = new List<PowderData>(powders);
        Shuffle(pool);
        var powderOffers = pool.GetRange(0, Mathf.Min(3, pool.Count));

        Debug.Log("화약 꾸러미 선택 시작");
        foreach (var ammo in candidates)
            Debug.Log($"카드 후보: {ammo}");

        foreach (var powder in powderOffers)
            Debug.Log($"화약 후보: {powder.name} ({powder.rarity})");
    }

    public void ApplyPowder(Ammo target, PowderData powder)
    {
        target.powder = powder;
        
    }

    // 파우더 가격.
    private int GetPowderPrice(Rare rerity) => rerity switch
    {
        // 케이스 문.
        Rare.Nomal => 1,
        Rare.Rare => 3,
        Rare.Unique => 5,
        Rare.Legendary => 7,
        _ => 0
    };
    // 유물 가격
    private int GetRelicPrice(Rare rarity) => rarity switch
    {
        Rare.Nomal => 5,
        Rare.Rare => 7,
        Rare.Unique => 9,
        Rare.Legendary => 11,
        _ => 0
    };
    // 셔플. (유틸)
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(0, i);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    
    [Serializable]
    public struct ShopItem
    {
        public ShopItemType type;
        public string name;
        public int price;
        public Ammo ammo;       // 탄환일 경우
        public RelicData relic; //유물일 경우

        public ShopItem(ShopItemType type, string name, int price , Ammo ammo, RelicData relic)
        {
            this.type = type;
            this.name = name;
            this.price = price;
            this.ammo = ammo;
            this.relic = relic;
        }
    }
    
}














