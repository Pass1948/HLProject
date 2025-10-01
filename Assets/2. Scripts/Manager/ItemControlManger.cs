using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using VHierarchy.Libs;

public class ItemControlManger : MonoBehaviour
{
    // ===== 가중치값 =====
    [Header("등급별 확률(백분율)")]
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
    [HideInInspector] public List<ItemModel> relicStatItems = new List<ItemModel>(); // 스탯증가부
    [HideInInspector] public List<ItemModel> relicRogicItems = new List<ItemModel>(); // 논리조건부
    [HideInInspector] public List<ItemModel> buyItems = new List<ItemModel>(); // 구매한 아이템 리스트
    // ===== 화약아이템 리스트 =====
    [HideInInspector] public List<ItemModel> powderItems = new List<ItemModel>();
    [HideInInspector] public List<ItemModel> powderStatItems = new List<ItemModel>(); // 스탯증가부
    [HideInInspector] public List<ItemModel> powderRogicItems = new List<ItemModel>(); // 논리조건부

    
    // ===== 아이템 Prefab을 위한 변수들 =====

    [HideInInspector]  public GameObject itemPrefab;
    [HideInInspector]   public GameObject relicRoot; // 구매한 아이템 보관함

    private Dictionary<string, BaseItem> itemsDictionary = new Dictionary<string, BaseItem>();

    [HideInInspector] public List<Ammo> drawPile = new(); // 구매한 총알 리스트
    [HideInInspector] public List<Ammo> discardPile = new();
    [HideInInspector] public List<Ammo> shopBullet = new();
    // =====================================================================
    // 아이템오브젝트 연동로직
    // =====================================================================

    public T GetItem<T>(GameObject host = null, string csName = null) where T : BaseItem
    {
        // 부착 대상 결정
        var attachTarget = host != null ? host : this.gameObject;

        // 딕셔너리 키(문자열 타입명이 있으면 그걸 사용)
        string key = string.IsNullOrEmpty(csName) ? typeof(T).Name : csName;

        //이미 등록되어 있으면 반환
        if (itemsDictionary.TryGetValue(key, out var existing) && existing != null)
            return existing as T ?? attachTarget.GetComponent<T>();

        BaseItem added = null;

        // 문자열 타입명이 넘어온 경우: 우선 그 타입으로 붙이기 시도
        if (!string.IsNullOrEmpty(csName))
        {
            System.Type t =
                // 풀네임 우선
                System.Type.GetType(csName) ??
                // 풀네임이 아니면 모든 어셈블리에서 클래스명으로 탐색
                System.Array.Find(
                    System.AppDomain.CurrentDomain.GetAssemblies(),
                    asm => asm.GetType(csName) != null
                )?.GetType(csName);

            if (t == null)
            {
                // 위 탐색이 실패했을 수 있으니 클래스명만으로 다시 한 번 전수 검사
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.Name == csName || type.FullName == csName)
                        {
                            t = type;
                            break;
                        }
                    }

                    if (t != null) break;
                }
            }

            if (t != null && typeof(BaseItem).IsAssignableFrom(t))
            {
                // 동일 타입 컴포넌트가 이미 붙어 있으면 재사용, 없으면 Add
                added = (BaseItem)(attachTarget.GetComponent(t) ?? attachTarget.AddComponent(t));
            }
        }

        // 문자열 타입으로 못 붙였거나 csName이 없으면 제네릭 T로 처리
        if (added == null)
        {
            added = attachTarget.GetComponent<T>();
            if (added == null) added = attachTarget.AddComponent<T>();
        }

        // 딕셔너리에 등록
        itemsDictionary[key] = added;

        // 반환 (문자열 타입으로 붙인 경우 T가 BaseItem일 것을 권장)
        return added as T ?? attachTarget.GetComponent<T>();
    }


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
        drawPile.Clear();
        buyItems.Clear();
        relicRoot.gameObject.Destroy();
        ItemDataSet();
    }

    // 화약과 유물을 가려내는 로직
    public void DivideItems(List<ItemModel> items)
    {
        relicItems.Clear();
        powderItems.Clear();
        if (items == null || items.Count == 0)
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
        if (items == null || items.Count == 0)
            return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].conditionall == 0) // 스택
            {
                relicStatItems.Add(items[i]);
            }

            if (items[i].conditionall == 1) // 로직
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
        if (items == null || items.Count == 0)
            return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].conditionall == 0) // 스택
            {
                powderStatItems.Add(items[i]);
            }

            if (items[i].conditionall == 1) // 로직
            {
                powderRogicItems.Add(items[i]);
            }
        }
    }

    // =====================================================================
    // 아이템 확률 관련 로직
    // =====================================================================
    // 불릿용 확률로직(5개 등급을 3~5개 불릿슬롯에 같은 확률(화약이 불릿에 적용될 확률)로 유지&등록)
    public List<Ammo> BulletWeightSampling(int slotCount) // 해당 메서드는 불릿에 효과 적용되는 걸로 설정
    {
        var result = new List<Ammo>();
        for (int r = 1; r <= slotCount; r++)
        {
            Suit fixedSuit = (Suit)UnityEngine.Random.Range(0, 4);
            int rank = UnityEngine.Random.Range(1, 14);
            result.Add(new Ammo { suit = fixedSuit, rank = rank });
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
            { RarityType.Normal, Mathf.Max(1f, normal) },
            { RarityType.Rare, Mathf.Max(1f, rare) },
            { RarityType.Elite, Mathf.Max(1f, elite) },
            { RarityType.Legendary, Mathf.Max(1f, legendary) },
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
    private void CreateItemObjects() // path로 생성될수있게 리팩토링 필요함
    {
        //유물 생성
        //유물은 ItemManager 밑에 붙이기(게임오버 or 종료시 relicRoot만 밀기)
    }

    // ========== 유물 슬롯과 이미지 생성 메서드 ==========
    // 해당 메서드 사용법 :
    // CreateRelicObject(사는 아이템.id);
    public void CreateRelicObject(int id, ItemModel item)
    {
        if (relicItems == null || relicItems.Count == 0) return;

        for (int i = 0; i < relicItems.Count; i++)
        {
            if (relicItems[i] == null) continue;
            if (relicItems[i].id == id)
            {
                var go = GameManager.Resource.Create<GameObject>(Path.ItemRelic + "RelicItem");
                go.name = relicItems[i].name;
                GetItem<BaseItem>(go, relicItems[i].csPath);
                go.transform.SetParent(relicRoot.transform, false);
                buyItems.Add(item);
                //go.gameObject.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>(Path.UISprites + relicItems[i].imagePath);
            }
        }
    }

    // ========== 유물 슬롯과 이미지 생성 메서드 ==========
    // 해당 메서드 사용법 :
    // CreateRelicObject(GameManager.ItemControl.PowderBundleWeightSampling(3), 여긴 layout달린 ui위치넣으셈);
    // private void CreatePowderObject(List<ItemModel> lists, Transform parent) // 유물 슬롯과 이미지 생성
    // {
    //     if (lists == null || lists.Count == 0) return;
    //
    //     for (int i = 0; i < lists.Count; i++)
    //     {
    //         if (lists[i] == null) continue;
    //
    //         var go = GameManager.Resource.Create<BaseItem>(Path.UIElements + "RelicIconUI");
    //         go.name = lists[i].name;
    //         go.transform.SetParent(parent, false);
    //         go.itemModel = lists[i];
    //         go.gameObject.GetComponent<Image>().sprite =
    //             GameManager.Resource.Load<Sprite>(Path.UISprites + lists[i].imagePath);
    //     }
    // }
}