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

    private static CommandManager commandManager;

    public static CommandManager Command => commandManager;

    private static CharacterManager characterManager;
    public static CharacterManager Character => characterManager;

    private static UIManager uiManager;
    public static UIManager UI => uiManager;

    private static EventManager eventManager;
    public static EventManager Event => eventManager;

    private static TurnBasedManager turnBasedManager;
    public static TurnBasedManager TurnBased => turnBasedManager;

    private static PathPreviewManager pathPreviewManager;
    public static PathPreviewManager PathPreview => pathPreviewManager;

    private static DataManager data;
    public static DataManager Data => data;


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
        commandManager = CreateChildManager<CommandManager>("CommandManager");
        characterManager = CreateChildManager<CharacterManager>("CharacterManager");
        uiManager = CreateChildManager<UIManager>("UIManager");
        eventManager = CreateChildManager<EventManager>("EventManager");
        turnBasedManager = CreateChildManager<TurnBasedManager>("TurnBasedManager");
        pathPreviewManager = CreateChildManager<PathPreviewManager>("PathPreviewManager");
        data = CreateChildManager<DataManager>("DataManager");
    }
    private T CreateChildManager<T>(string goName) where T : Component
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform, false);
        return go.AddComponent<T>();
    }

}
