using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTable;
using UnityEngine;

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
    [HideInInspector] public List<BaseItem> items = new List<BaseItem>(); // 모든 아이템 리스트
    private BaseItem baseItem;

    [HideInInspector]
    public BaseItem BaseItem
    {
        get { return baseItem; }
        set { baseItem = value; }
    }

    // ===== 유물아이템 리스트 =====
    [HideInInspector] public List<BaseItem> relicItems = new List<BaseItem>();

    // ===== 화약아이템 리스트 =====
    [HideInInspector] public List<BaseItem> powderItems = new List<BaseItem>();

    // =====================================================================
    // 아이템 데이터 관련 로직
    // =====================================================================
    public void ItemDataSet() // 아이템 리스트에 데이터 입력(데이터매니저에서 호출)
    {
        var list = RelicDataGroup.GetList();

        Debug.Log($"지금 유물의 갯수는?{list.Count()}");

        for (int i = 0; i < list.Count; i++)
        {
            BaseItem.InitItem(GameManager.Data.relicDataGroup.GetRelicData(list[i].id));
        }

        DivideItems(items);
    }

    public void ClearData() // 아이템 리스트를 비우고 새롭게 할당할경우 사용
    {
        items.Clear();
        ItemDataSet();
    }

    // 화약과 유물을 가려내는 로직
    public void DivideItems(List<BaseItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemModel.itemType == ItemType.Relic)
            {
                relicItems.Add(items[i]);
            }

            if (items[i].itemModel.itemType == ItemType.GunPowder)
            {
                powderItems.Add(items[i]);
            }
        }
    }

    // =====================================================================
    // 아이템 확률 관련 로직
    // =====================================================================
    // 불릿용 확률로직(5개 등급을 3~5개 불릿슬롯에 같은 확률(화약이 불릿에 적용될 확률)로 유지&등록)
    public List<BaseItem> BulletWeightSampling(List<BaseItem> powders, int slotCount)
    {
        var result = new List<BaseItem>();
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
            var pool = FilterByRarity(powders, pickedRarity);
            if (pool.Count == 0) // 해당 등급이 비었으면 폴백
            {
                if (powders.Count == 0) break;
                result.Add(powders[Random.Range(0, powders.Count)]);
            }
            else
            {
                result.Add(pool[Random.Range(0, pool.Count)]);
            }
        }
        return result;
    }

    // 유물아이템용 확률로직(2개슬롯에 4개 등급(common제외) 확률 등록)
    public List<BaseItem> RelicWeightSampling(List<BaseItem> relics, int slotCount)
    {
        var result = new List<BaseItem>();
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
            var pool = FilterByRarity(relics, pickedRarity);

            if (pool.Count == 0)
            {
                result.Add(relicItems[Random.Range(0, relics.Count)]);
            }
            else
            {
                result.Add(pool[Random.Range(0, pool.Count)]);
            }
        }

        return result;
        
    }

    // 화약아이템용 확률로직(2개슬롯에 4개 등급(common제외) 확률 등록) 
    // 화약주머니 메서드(3개 들어감)
    public List<BaseItem> PowderWeightSampling(List<BaseItem> powders, int count)
    {
        var result = new List<BaseItem>();
        if (relicItems == null || relicItems.Count == 0)
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
            var pool = FilterByRarity(powders, pickedRarity);

            if (pool.Count == 0)
            {
                result.Add(relicItems[Random.Range(0, powders.Count)]);
            }
            else
            {
                result.Add(pool[Random.Range(0, pool.Count)]);
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
            a += w.Value;
            if (roll <= a) return w.Key;
        }

        return weights.Keys.Last();
    }

    // 특정 등급의 아이템 풀
    private List<BaseItem> FilterByRarity(List<BaseItem> pool, RarityType rarity)
    {
        var list = new List<BaseItem>();
        for (int i = 0; i < pool.Count; i++)
        {
            var it = pool[i];
            if (it == null || it.itemModel == null) continue;
            if (it.itemModel.rarity == rarity)
                list.Add(it);
        }

        return list;
    }
}