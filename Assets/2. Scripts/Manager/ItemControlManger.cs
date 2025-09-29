using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemControlManger : MonoBehaviour
{
    // ===== 가중치값 =====
    [SerializeField] private float common = 70f; // 70%
    [SerializeField] private float normal = 20f; // 20%, 65%
    [SerializeField] private float rare = 5f; // 5%, 24%
    [SerializeField] private float elite = 4; // 4%, 10%
    [SerializeField] private float legendary = 1f; // 1%, 1%

    // ===== 유물/화약을 필터링할시 필요한 id값 =====
    [HideInInspector] public int relicID = 3001; // 유물 초기 세팅시 필요한 첫번째 id
    [HideInInspector] public int gunPowderID = 3501; // 화약 초기 세팅시 필요한 첫번째 id

    // ===== 모든 아이템 리스트에 필요한 값 =====
    [HideInInspector] public List<ItemModel> items = new List<ItemModel>(); // 모든 아이템 리스트
    private ItemModel[] ItemIds;
    
    // ===== 유물아이템 리스트 =====
    [HideInInspector] public List<ItemModel> relicItems = new List<ItemModel>();
    [HideInInspector] public List<ItemModel> relicStatItems = new List<ItemModel>();    // 스탯증가부
    [HideInInspector] public List<ItemModel> relicRogicItems = new List<ItemModel>();    // 논리조건부

    // ===== 화약아이템 리스트 =====
    [HideInInspector] public List<ItemModel> powderItems = new List<ItemModel>();
    [HideInInspector] public List<ItemModel> powderStatItems = new List<ItemModel>();    // 스탯증가부
    [HideInInspector] public List<ItemModel> powderRogicItems = new List<ItemModel>();    // 논리조건부
    
    // ===== 아이템 Prefab을 위한 변수들 =====
    
    public GameObject itemPrefab;
    public GameObject relicRoot;// 구매한 아이템 보관함
    
    private Dictionary<string, BaseItem> itemsDictionary = new Dictionary<string, BaseItem>();
    
    
    // =====================================================================
    // 아이템 데이터 관련 로직
    // =====================================================================
    public void ItemDataSet() // 아이템 리스트에 데이터 입력(데이터매니저에서 호출)
    {
        // 정리용 오브젝트
        (relicRoot = new GameObject("RelicRoot")).transform.SetParent(transform);   
        
        // 데이터 리스트 생성
        var list = RelicDataGroup.GetList();
        
        // 최대 ID 기준으로 배열 사이즈 결정 (희소 ID도 null로 둠)
        int maxId = list.Max(r => r.id);
        ItemIds = new ItemModel[maxId + 1];
        
        items.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            var m = new ItemModel();
            m.InitData(list[i]); // 스프레드시트 → 런타임 모델 매핑
            items.Add(m);
            if (m.id >= 0 && m.id < ItemIds.Length) 
                ItemIds[m.id] = m; // 희소 ID 안전
        }
        DivideItems(items);

    }

    public void ClearData() // 아이템 리스트를 비우고 새롭게 할당할경우 사용
    {
        items.Clear();
        ItemIds = null;
        relicItems.Clear();
        powderItems.Clear();
        ItemDataSet();
    }

    // 화약과 유물을 가려내는 로직
    public void DivideItems(List<ItemModel> items)
    {
        relicItems.Clear();
        powderItems.Clear();
        if(items == null || items.Count == 0)
            return;
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == ItemType.Relic)
            {
                relicItems.Add(items[i]);
            }

            if (items[i].itemType == ItemType.GunPowder)
            {
                powderItems.Add(items[i]);
            }
        }
        DivideRelicItems(relicItems);
        DividePowderItems(powderItems);
    }
    
    // (전략패턴전용) 유물아이템 로직부와 스탯부 나누기
    public void DivideRelicItems(List<ItemModel> items)
    {
        relicStatItems.Clear();
        relicRogicItems.Clear();
        if(items == null || items.Count == 0)
            return;
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].conditionall == 0) // 스택
            {
                relicStatItems.Add(items[i]);
            }

            if (items[i].conditionall == 1)// 로직
            {
                relicRogicItems.Add(items[i]);
            }
        }
    }
    // (전략패턴전용) 화약아이템 로직부와 스탯부 나누기
    public void DividePowderItems(List<ItemModel> items)
    {
        powderStatItems.Clear();
        powderRogicItems.Clear();
        if(items == null || items.Count == 0)
            return;
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].conditionall == 0) // 스택
            {
                powderStatItems.Add(items[i]);
            }

            if (items[i].conditionall == 1)// 로직
            {
                powderRogicItems.Add(items[i]);
            }
        }
    }
    
    // =====================================================================
    // 아이템 확률 관련 로직
    // =====================================================================
    // 불릿용 확률로직(5개 등급을 3~5개 불릿슬롯에 같은 확률(화약이 불릿에 적용될 확률)로 유지&등록)
    public List<ItemModel> BulletWeightSampling(int slotCount)  // 해당 메서드는 불릿에 효과 적용되는 걸로 설정
    {
        var result = new List<ItemModel>();
        var weights = new Dictionary<RarityType, float>
        {
            { RarityType.Common, Mathf.Max(0f, common) },
            { RarityType.Normal, Mathf.Max(0f, normal) },
            { RarityType.Rare, Mathf.Max(0f, rare) },
            { RarityType.Elite, Mathf.Max(0f, elite) },
            { RarityType.Legendary, Mathf.Max(0f, legendary) },
        };

        for (int i = 0; i < slotCount; i++)
        {
            var pickedRarity = PickRarity(weights);
            var pool = FilterByRarity(powderItems, pickedRarity);
            if (pool.Count == 0) // 해당 등급이 비었으면 폴백
            {
                if (powderItems.Count == 0) break;
                result.Add(powderItems[UnityEngine.Random.Range(0, powderItems.Count)]);
            }
            else
            {
                result.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
            }
        }
        return result;
    }

    // 유물아이템용 확률로직(2개슬롯에 4개 등급(common제외) 확률 등록)
    public List<ItemModel> RelicWeightSampling(int slotCount)
    {
        var result = new List<ItemModel>();
        if (relicItems == null || relicItems.Count == 0)
            return result;
        var weights = new Dictionary<RarityType, float>
        {
            { RarityType.Normal, Mathf.Max(0f, normal) },
            { RarityType.Rare, Mathf.Max(0f, rare) },
            { RarityType.Elite, Mathf.Max(0f, elite) },
            { RarityType.Legendary, Mathf.Max(0f, legendary) },
        };
        for (int i = 0; i < slotCount; i++)
        {
            var pickedRarity = PickRarity(weights);
            var pool = FilterByRarity(relicItems, pickedRarity);

            if (pool.Count == 0)
            {
                result.Add(relicItems[UnityEngine.Random.Range(0, relicItems.Count)]);
            }
            else
            {
                result.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
            }
        }

        return result;
    }

    // 화약아이템용 확률로직(3개 화약에 4개 등급(common제외) 확률 등록) 
    public List<ItemModel> PowderBundleWeightSampling(int count)
    {
        var result = new List<ItemModel>();
        if (powderItems == null || powderItems.Count == 0)
            return result;
        var weights = new Dictionary<RarityType, float>
        {
            { RarityType.Normal, Mathf.Max(0f, normal) },
            { RarityType.Rare, Mathf.Max(0f, rare) },
            { RarityType.Elite, Mathf.Max(0f, elite) },
            { RarityType.Legendary, Mathf.Max(0f, legendary) },
        };
        for (int i = 0; i < count; i++)
        {
            var pickedRarity = PickRarity(weights);
            var pool = FilterByRarity(powderItems, pickedRarity);

            if (pool.Count == 0)
            {
                result.Add(relicItems[UnityEngine.Random.Range(0, powderItems.Count)]);
            }
            else
            {
                result.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
            }
        }

        return result;

    }

    // 등급 선택
    private RarityType PickRarity(Dictionary<RarityType, float> weights)
    {
        // 총합
        float total = 0f;
        foreach (var w in weights) total += w.Value; // 100%

        // 총합이 0f 일경우 첫키를 반환하도록 안전처리
        if (total <= 0f) return weights.Keys.First();
        
        float roll = UnityEngine.Random.Range(0f, total);
        float a = 0f;
        foreach (var w in weights)
        {
            a += Mathf.Max(0f, w.Value);
            if (roll <= a) return w.Key;
        }

        return weights.Keys.Last();
    }

    // 특정 등급의 아이템 풀
    private List<ItemModel> FilterByRarity(List<ItemModel> pool, RarityType rarity)
    {
        var list = new List<ItemModel>();
        if (pool == null) return list;
        for (int i = 0; i < pool.Count; i++)
        {
            var it = pool[i];
            if (it != null && it.rarity == rarity) 
                list.Add(it);
        }

        return list;
    }
    
    // =====================================================================
    // 아이템 오브젝트 생성
    // =====================================================================
    private void CreateItemObjects()    // path로 생성될수있게 리팩토링 필요함
    {
        //유물 생성
        //유물은 ItemManager 밑에 붙이기(게임오버 or 종료시 relicRoot만 밀기)
    }
    
    // ========== 유물 슬롯과 이미지 생성 메서드 ==========
    // 해당 메서드 사용법 :
    // CreateRelicObject(GameManager.ItemControl.RelicWeightSampling(4), 여긴 layout달린 ui위치넣으셈);
    private void CreateRelicObject(List<ItemModel> lists, Transform parent) 
    {
        if (lists == null || lists.Count == 0) return;

        for (int i = 0; i < lists.Count; i++)
        {
            if (lists[i] == null) continue;
            
               var go = GameManager.Resource.Create<BaseItem>(Path.UIElements+"RelicIconUI");
               go.name = lists[i].name;
               go.transform.SetParent(parent, false);
               go.itemModel = lists[i];
               go.gameObject.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>(Path.UISprites+lists[i].path);
        }
    }
    // ========== 유물 슬롯과 이미지 생성 메서드 ==========
    // 해당 메서드 사용법 :
    // CreateRelicObject(GameManager.ItemControl.PowderBundleWeightSampling(3), 여긴 layout달린 ui위치넣으셈);
    private void CreatePowderObject(List<ItemModel> lists, Transform parent) // 유물 슬롯과 이미지 생성
    {
        if (lists == null || lists.Count == 0) return;

        for (int i = 0; i < lists.Count; i++)
        {
            if (lists[i] == null) continue;
            
            var go = GameManager.Resource.Create<BaseItem>(Path.UIElements+"RelicIconUI");
            go.name = lists[i].name;
            go.transform.SetParent(parent, false);
            go.itemModel = lists[i];
            go.gameObject.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>(Path.UISprites+lists[i].path);
        }
    }

    // =====================================================================
    // 아이템 데이터 관련 로직
    // =====================================================================
    
    
}