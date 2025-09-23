using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]       // GameManager가 다른 Script보다 먼저 호출되게 설정
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    // Managers=========================

    private static ResourceManager resourceManager;
    public static ResourceManager Resource => resourceManager;

    private static SceneLoadManager sceneLoadManager;
    public static SceneLoadManager SceneLoad => sceneLoadManager;

    private static UnitManager unitManager;
    public static UnitManager Unit => unitManager;

    private static UIManager uiManager;
    public static UIManager UI => uiManager;

    private static EventManager eventManager;
    public static EventManager Event => eventManager;

    private static TurnBasedManager turnBasedManager;
    public static TurnBasedManager TurnBased => turnBasedManager;

    private static DataManager data;
    public static DataManager Data => data;

    private static MapManager mapManager;
    public static MapManager Map => mapManager;

    private static MouseManager mouseManager;
    public static MouseManager Mouse => mouseManager;
    
    private static SaveLoadManager saveLoadManager;
    public static SaveLoadManager SaveLoad => saveLoadManager;
    
    private static ItemControlManger itemControlManger;
    public static ItemControlManger ItemControl => itemControlManger;

    private static CurrencyManager currency;
    public static CurrencyManager Currency => currency;
    
    private static RewardManager reward;
    public static RewardManager Reward => reward;


    private void Awake()
    {
        if (instance != null) { Destroy(this); return; }
        instance = this;
        DontDestroyOnLoad(this);
        InitManagers();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void InitManagers()
    {
        resourceManager = CreateChildManager<ResourceManager>("ResourceManager");
        sceneLoadManager = CreateChildManager<SceneLoadManager>("SceneLoadManager");
        unitManager = CreateChildManager<UnitManager>("UnitManager");
        uiManager = CreateChildManager<UIManager>("UIManager");
        eventManager = CreateChildManager<EventManager>("EventManager");
        turnBasedManager = CreateChildManager<TurnBasedManager>("TurnBasedManager");
        data = CreateChildManager<DataManager>("DataManager");
        mapManager = CreateChildManager<MapManager>("MapManager");
        mouseManager = CreateChildManager<MouseManager>("MouseManager");
        saveLoadManager = CreateChildManager<SaveLoadManager>("SaveLoadManager");
        itemControlManger = CreateChildManager<ItemControlManger>("ItemControlManger");
        currency = CreateChildManager<CurrencyManager>("CurrencyManager");
        reward = CreateChildManager<RewardManager>("RewardManager");
    }
    private T CreateChildManager<T>(string goName) where T : Component
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform, false);
        return go.AddComponent<T>();
    }

}
