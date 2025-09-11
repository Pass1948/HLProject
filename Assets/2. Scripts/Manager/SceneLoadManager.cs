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
        // �� Ŭ������ enum ����
        _scenes.Add(SceneType.Test, new TestScene());
    }

    public void LoadScene(SceneType sceneType)
    {
        // �������� �� �ε��� ������ ����
        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);

        // �ε��� ���� ��ϵǾ����� Ȯ��
        if (!_scenes.TryGetValue(sceneType, out var scene))
        {
            Debug.LogError($"SceneType �� �����ϴ�. : {sceneType}");
            return;
        }

        // �ε��� ���� ���� ���� ������ Ȯ��
        if (_currentScene == scene) return;

        _loadingCoroutine = StartCoroutine(LoadSceneProcess(sceneType));
    }

    IEnumerator LoadSceneProcess(SceneType sceneType)
    {
        // �� Ÿ�Կ� ���� �� ������Ʈ
        // ���� ���� �ִٸ� ���� �ݹ� ����
        var scene = _scenes[sceneType];
        _currentScene?.SceneExit();

        // ���� ���� ���� ���� ����� ��ȯ
        _prevScene = _currentScene;
        _currentScene = scene;
        _currentSceneType = sceneType;

        // �� �ε� ����
        var operation = SceneManager.LoadSceneAsync(sceneType.ToString());
        operation.allowSceneActivation = false;

        // �� �ε��ϸ鼭 �ҷ��� �����Ͱ� ������ �ε�
        // API �� ��巹������� �񵿱� ����� ���ϰ� �־ ���������� ���ʿ�
        _currentScene.SceneLoading();

        // operation.allowSceneActivation �� false �ϋ��� 0.9 ������ �����
        while (operation.progress < 0.9f)
            yield return null;

        // �� ��ȯ �㰡
        operation.allowSceneActivation = true;

        // �� ��ȯ �Ϸ� ��ٸ���
        while (!operation.isDone)
            yield return null;

        // �������� ��ȯ ���
        // (���� �ִ� ������Ʈ - OnEnable / Awake / Start �� �ʱ�ȭ ���) 
        yield return null;

        // �ε��� ���� ���� �ݹ� ����
        _currentScene.SceneEnter();
        _loadingCoroutine = null;
    }

    public void RestartScene()
    {
        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);

        // Ÿ�ӽ������� 0�� �� ������ ����
        if (Time.timeScale == 0f) Time.timeScale = 1f;

        // ���� �� �ٽ� �ε�
        _loadingCoroutine = StartCoroutine(LoadSceneProcess(_currentSceneType));
    }
}
