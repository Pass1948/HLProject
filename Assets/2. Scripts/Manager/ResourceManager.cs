using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager : MonoBehaviour
{
    // ĳ�̿� ��ųʸ� (�ѹ� �ε��� ������ ���� �����ؼ� �����Ѵ�.)
    private Dictionary<string, Object> _cache = new();
    private Dictionary<string, List<Object>> _labelCache = new();
    // key ������� Addressables���� ���� �ε�
    public async Task<T> LoadAssetAsync<T>(string key) where T : Object
    {
        // �̹� ĳ�ÿ� ������ ��� ��ȯ�Ѵ�.
        if (_cache.TryGetValue(key, out var cached)) 
            return cached as T;

        // ������ Addresables���� �ε� �� ĳ�ÿ� ����
        var handle = Addressables.LoadAssetAsync<T>(key);
        var asset = await handle.Task;

        if (asset != null) 
            _cache[key] = asset;

        return asset;
    }

    // label ������� Addressables���� ���� �ε�
    public async Task<IList<T>> LoadByLabelAsync<T> (string label) where T : Object
    {
        // �ҷ��� ���µ��� ĳ��(��ųʸ�)�� ���
        var handle = Addressables.LoadAssetsAsync<T>(label, obj =>
        {
            if(!_cache.ContainsKey(obj.name))
                _cache[obj.name] = obj;

            if (!_labelCache.ContainsKey(label))
                _labelCache[label] = new List<Object>();

            if (!_labelCache.ContainsKey(obj.name))
                _labelCache[label].Add(obj);

        });

        // �ε�� ���µ��� ����Ʈ
        return await handle.Task;
    }

    // ĳ�� Ȥ�� Addressable���� ���� �������� (14��° �� �޼���) - Ű ������� prefab ������ �����´�.
    public async Task<T> Get<T>(string key) where T : Object
    {
        return await LoadAssetAsync<T>(key);
    }

    // ĳ�� Ȥ�� Addressable���� ���� �����ͼ� Instantiate ���ش� - Ű ������� prefab ������ �����ͼ� ����.
    public async Task<T> Create<T> (string key, Transform parent = null) where T : Object
    {
        var prefab = await LoadAssetAsync<T>(key);
        return prefab != null ? Instantiate(prefab, parent) : null;
    }

    // ��(label) �ȿ��� Ư�� prefabName�� ���� �������� - �� ��ü�� �ҷ��� ��, ���ϴ� �̸�(prefabName)�� ��ȯ
    // prefab ������ ������ �� ���
    public async Task<T> GetLabel<T>(string label, string prefabName) where T : Object
    {
        if (!_cache.ContainsKey(prefabName))
            await LoadByLabelAsync<T>(label);

        return _cache.TryGetValue(prefabName, out var prefab) ? prefab as T : null;
    }

    // ��(label) �ȿ��� Ư�� prefabName�� ������ ã�� Instantiate
    // prefab�� ��� �����ؼ� ��ȯ
    public async Task<T> CreateLabel<T>(string label, string prefabName, Transform parent = null) where T : Object
    {
        var prefab = await GetLabel<T>(label, prefabName);
        return prefab != null ? Instantiate(prefab, parent) : null;

    }

    public async Task<List<T>> GetLabelAll<T>(string label) where T : Object
    {
        // ���� ���� ĳ�ÿ� ���ٸ� �ڵ����� LoadByLabelAsync ȣ��
        if (!_labelCache.ContainsKey(label))
            await LoadByLabelAsync<T>(label);

        var result = new List<T>();
        foreach (var prefab in _labelCache[label])
        {
            if (prefab is T casted)
                result.Add(casted);
        }
        return result;
    }

    private async Task<List<T>> CreateLabelAllAsync<T>(string label, Transform parent = null) where T : Object
    {
        // GetByLabel�� ����
        var prefabs = await GetLabelAll<T>(label);
        if (prefabs == null) return null;

        var instances = new List<T>();
        foreach (var prefab in prefabs)
        {
            var obj = Instantiate(prefab, parent);
            instances.Add(obj);
        }
        return instances;
    }

    public List<T> CreateLabelAll<T>(string label, Transform parent = null) where T : Object
    {
        var task = CreateLabelAllAsync<T>(label, parent);
        task.Wait(); // Task�� ���� ������ ���

        return task.Result; // List<T> ��ȯ
    }
}
