using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public T Load<T>(string prefName) where T : Object
    {
        return Resources.Load<T>(prefName);
    }

    public T Create<T>(string path, Transform parent = null) where T : Object
    {
        T res = Load<T>(path);
        if (res == null)
        {
            Debug.Log($"프리팹이 없습니다. : {path}");
            return default;
        }

        T obj = Instantiate(res, parent);
        return obj;
    }

    public T Create<T>(string prefKey, string path, Transform parent = null) where T : Object
    {
        string key = prefKey + path;
        return Create<T>(key, parent);
    }

    public T CreateCharacter<T>(string prefName, Transform parent = null) where T : Object
    {
        return Create<T>(Path.Character, prefName, parent);
    }

    public T CreateEnemy<T>(string prefName, Transform parent = null) where T : Object
    {
        return Create<T>(Path.Enemy, prefName, parent);
    }

    public T CreateItem<T>(string prefName, Transform parent = null) where T : Object
    {
        return Create<T>(Path.Item, prefName, parent);
    }

    public T CreateMap<T>(string prefName, Transform parent = null) where T : Object
    {
        return Create<T>(Path.Map, prefName, parent);
    }

    public T CreateUI<T>(string prefName, Transform parent = null) where T : Object
    {
        return Create<T>(Path.UI, prefName, parent);
    }


}
