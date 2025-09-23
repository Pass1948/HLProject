using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<RelicData> relics;
    
    
    public Rare rare;
    public List<ShopItem> offers = new(); // 상점 카드 오퍼
    
    
    
    public int rerollCount = 2; // 리롤 카운트,코스트
    public int rerollCost = 1;

    private int deckCount = 3;

    private int playerGold; //달란티

    private void Start()
    {
        playerGold = GameManager.Unit.Player.playerHandler.playerMonney;
        ShopStart();
    }
//플레이어 모델 ㅡ게임메니저,데이터 메니저

    public void ShopStart()
    {
        offers.Clear();
        rerollCount = 2;
        rerollCost = 1;
    }

    public void TryBuy(int index)
    {
        if (index < 0 || index >= rerollCount) return;
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
                Debug.Log("자 회복 아이템을 샀네요"); // 사면 바로 힐되면 됨
                break;
            case ShopItemType.RemoveBullet:
                Debug.Log("자 총알 지우기를 샀네요"); // 총알 
                break;
        }
    }
    

    public struct ShopItem
    {
        public ShopItemType type;
        public string name;
        public int price;

        public ShopItem(ShopItemType type, string name, int price)
        {
            this.type = type;
            this.name = name;
            this.price = price;
        }
    }
    
}














