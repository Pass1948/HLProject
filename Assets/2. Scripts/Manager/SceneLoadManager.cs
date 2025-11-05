using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{

    private Dictionary<SceneType, BaseScene> _scenes = new();
    private BaseScene _prevScene;
    private BaseScene _currentScene;
    private SceneType _currentSceneType;

    Coroutine _loadingCoroutine;

    private void Awake()
    {
        _scenes.Add(SceneType.Title, new TitleScene());
        _scenes.Add(SceneType.GameScene, new TestScene());
    }

    public void LoadScene(SceneType sceneType)
    {
        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);
        
        if (!_scenes.TryGetValue(sceneType, out var scene))
        {
            Debug.LogError($"SceneType 이 없습니다. : {sceneType}");
            return;
        }

        if (_currentScene == scene) return;

        _loadingCoroutine = StartCoroutine(LoadSceneProcess(sceneType));
    }

    IEnumerator LoadSceneProcess(SceneType sceneType)
    {
        GameManager.UI.OpenUI<LoadingUI>();
        
        var scene = _scenes[sceneType];
        _currentScene?.SceneExit();
        
        _prevScene = _currentScene;
        _currentScene = scene;
        _currentSceneType = sceneType;
        
        var operation = SceneManager.LoadSceneAsync(sceneType.ToString());
        operation.allowSceneActivation = false;

        _currentScene.SceneLoading();

        while (operation.progress < 0.9f)
            yield return null;
        
        operation.allowSceneActivation = true;
        
        while (!operation.isDone)
            yield return null;
        
        yield return null;
        
        _currentScene.SceneEnter();
        _loadingCoroutine = null;
    }

    public void RestartScene()
    {
        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);

        if (Time.timeScale == 0f) Time.timeScale = 1f;

        _loadingCoroutine = StartCoroutine(LoadSceneProcess(_currentSceneType));
    }
}
