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

    private static DataManager dataManager;
    public static DataManager Data => dataManager;
    
    private static ResourceManager resourceManager;
    public static ResourceManager Resource => resourceManager;

    private static SceneLoadManager sceneLoadManager;
    public static SceneLoadManager SceneLoad => sceneLoadManager;

    private static StageManager stageManager;
    public static StageManager Stage => stageManager;
    
    private static UnitManager unitManager;
    public static UnitManager Unit => unitManager;

    private static UIManager uiManager;
    public static UIManager UI => uiManager;

    private static EventManager eventManager;
    public static EventManager Event => eventManager;

    private static TurnBasedManager turnBasedManager;
    public static TurnBasedManager TurnBased => turnBasedManager;

    private static MapManager mapManager;
    public static MapManager Map => mapManager;

    private static MouseManager mouseManager;
    public static MouseManager Mouse => mouseManager;
    
    private static SaveLoadManager saveLoadManager;
    public static SaveLoadManager SaveLoad => saveLoadManager;
    
    private static ItemControlManger itemControlManger;
    public static ItemControlManger ItemControl => itemControlManger;

    private static ShopManager shopManager;
    public static ShopManager Shop => shopManager;
    
    private static SoundManager soundManager;
    public static SoundManager Sound => soundManager;


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
        dataManager = CreateChildManager<DataManager>("DataManager");
        dataManager.Initialize();
        resourceManager = CreateChildManager<ResourceManager>("ResourceManager");
        sceneLoadManager = CreateChildManager<SceneLoadManager>("SceneLoadManager");
        stageManager = CreateChildManager<StageManager>("StageManager");
        unitManager = CreateChildManager<UnitManager>("UnitManager");
        uiManager = CreateChildManager<UIManager>("UIManager");
        eventManager = CreateChildManager<EventManager>("EventManager");
        turnBasedManager = CreateChildManager<TurnBasedManager>("TurnBasedManager");
        mapManager = CreateChildManager<MapManager>("MapManager");
        mouseManager = CreateChildManager<MouseManager>("MouseManager");
        saveLoadManager = CreateChildManager<SaveLoadManager>("SaveLoadManager");
        itemControlManger = CreateChildManager<ItemControlManger>("ItemControlManger");
        shopManager = CreateChildManager<ShopManager>("ShopManager");
        soundManager = CreateChildManager<SoundManager>("SoundManager");
    }
    private T CreateChildManager<T>(string goName) where T : Component
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform, false);
        return go.AddComponent<T>();
    }
}
