  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public const string UICommonPath = "Common/";
    public const string UIPrefabPath = "Elements/";

    private Transform _uiRoot;
    private EventSystem _eventSystem;

    private bool _isCleaning;
    private Dictionary<string, BaseUI> _uiDictionary = new Dictionary<string, BaseUI>();

    public bool ShowGuide = true;

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OpenUI<T>() where T : BaseUI
    {
        var ui = GetUI<T>();
        ui?.OpenUI();
    }

    public void CloseUI<T>() where T : BaseUI
    {
        if (IsExistUI<T>())
        {
            var ui = GetUI<T>();
            ui?.CloseUI();
        }
    }

    public T GetUI<T>() where T : BaseUI
    {
        if (_isCleaning) return null;

        string uiName = GetUIName<T>();

        BaseUI ui;
        if (IsExistUI<T>())
            ui = _uiDictionary[uiName];
        else
            ui = CreateUI<T>();

        return ui as T;
    }

    private T CreateUI<T>() where T : BaseUI
    {
        if (_isCleaning) return null;

        string uiName = GetUIName<T>();
        if (_uiDictionary.TryGetValue(uiName, out var prevUi) && prevUi != null)
        {
            Destroy(prevUi.gameObject);
            _uiDictionary.Remove(uiName);
        }

        CheckCanvas();
        CheckEventSystem();

        string path = GetPath<T>();
        GameObject prefab = GameManager.Resource.CreateUI<GameObject>(path, _uiRoot);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] Prefab not found: {path}");
            return null;
        }

        T ui = prefab.GetComponent<T>();
        if (ui == null)
        {
            Debug.LogError($"[UIManager] Prefab has no component : {uiName}");
            Destroy(prefab);
            return null;
        }

        // 3. Dictionary ���
        _uiDictionary[uiName] = ui;

        return ui;
    }

    public T CreateSlotUI<T>(Transform parent = null) where T : BaseUI
    {
        if (_isCleaning) return null;


        string path = GetPath<T>();
        T prefab = GameManager.Resource.CreateUI<T>(path, parent);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] Prefab not found: {path}");
            return null;
        }

        return prefab;
    }

    public bool IsExistUI<T>() where T : BaseUI
    {
        string uiName = GetUIName<T>();
        return _uiDictionary.TryGetValue(uiName, out var ui) && ui != null;
    }
    
    private string GetPath<T>() where T : BaseUI
    {
        return UIPrefabPath + GetUIName<T>();
    }

    private string GetUIName<T>() where T : BaseUI 
    {
        return typeof(T).Name;
    }
    
    private void CheckCanvas()
    {
        if (_uiRoot != null)
            return;

        string prefKey = Path.UI + UICommonPath + Prefab.Canvas;
        _uiRoot = GameManager.Resource.Create<Transform>(prefKey);
    }

    private void CheckEventSystem()
    {
        if (_eventSystem != null)
            return;

        string prefKey = Path.UI + UICommonPath + Prefab.EventSystem;
        _eventSystem = GameManager.Resource.Create<EventSystem>(prefKey);
    }
    
    private void OnSceneUnloaded(Scene scene)
    {
        CleanAllUIs();
        StartCoroutine(CoUnloadUnusedAssets());
    }

    private void CleanAllUIs()
    {
        if (_isCleaning) return;
        _isCleaning = true;

        try
        {
            foreach (var ui in _uiDictionary.Values)
            {
                if (ui == null) continue;
                Destroy(ui.gameObject);
            }
            _uiDictionary.Clear();
        }
        finally
        {
            _isCleaning = false;
        }
    }
    
    private IEnumerator CoUnloadUnusedAssets()
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
