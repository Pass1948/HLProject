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
    public static SceneLoadManager SceneLoadManager => sceneLoadManager;

    private static CommandManager commandManager;
    public static CommandManager Command => commandManager;

    private static CharacterManager characterManager;
    public static CharacterManager Character => characterManager;

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

    }
    private T CreateChildManager<T>(string goName) where T : Component
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform, false);
        return go.AddComponent<T>();
    }

}
