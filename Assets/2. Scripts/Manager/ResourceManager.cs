using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager : MonoBehaviour
{
    // 캐싱용 딕셔너리 (한번 로드한 에셋은 여기 저장해서 재사용한다.)
    private Dictionary<string, Object> _cache = new();
    private Dictionary<string, List<Object>> _labelCache = new();
    // key 기반으로 Addressables에서 에셋 로드
    public async Task<T> LoadAssetAsync<T>(string key) where T : Object
    {
        // 이미 캐시에 있으면 즉시 반환한다.
        if (_cache.TryGetValue(key, out var cached)) 
            return cached as T;

        // 없으면 Addresables에서 로드 후 캐시에 저장
        var handle = Addressables.LoadAssetAsync<T>(key);
        var asset = await handle.Task;

        if (asset != null) 
            _cache[key] = asset;

        return asset;
    }

    // label 기반으로 Addressables에서 에셋 로드
    public async Task<IList<T>> LoadByLabelAsync<T> (string label) where T : Object
    {
        // 불러온 에셋들을 캐시(딕셔너리)에 등록
        var handle = Addressables.LoadAssetsAsync<T>(label, obj =>
        {
            if(!_cache.ContainsKey(obj.name))
                _cache[obj.name] = obj;

            if (!_labelCache.ContainsKey(label))
                _labelCache[label] = new List<Object>();

            if (!_labelCache.ContainsKey(obj.name))
                _labelCache[label].Add(obj);

        });

        // 로드된 에셋들의 리스트
        return await handle.Task;
    }

    // 캐시 혹은 Addressable에서 에셋 가져오기 (14번째 줄 메서드) - 키 기반으로 prefab 원본을 가져온다.
    public async Task<T> Get<T>(string key) where T : Object
    {
        return await LoadAssetAsync<T>(key);
    }

    // 캐시 혹은 Addressable에서 에셋 가져와서 Instantiate 해준다 - 키 기반으로 prefab 원본을 가져와서 생성.
    public async Task<T> Create<T> (string key, Transform parent = null) where T : Object
    {
        var prefab = await LoadAssetAsync<T>(key);
        return prefab != null ? Instantiate(prefab, parent) : null;
    }

    // 라벨(label) 안에서 특정 prefabName의 에셋 가져오기 - 라벨 전체를 불러온 뒤, 원하는 이름(prefabName)만 반환
    // prefab 원본만 가져올 때 사용
    public async Task<T> GetLabel<T>(string label, string prefabName) where T : Object
    {
        if (!_cache.ContainsKey(prefabName))
            await LoadByLabelAsync<T>(label);

        return _cache.TryGetValue(prefabName, out var prefab) ? prefab as T : null;
    }

    // 라벨(label) 안에서 특정 prefabName의 에셋을 찾아 Instantiate
    // prefab을 즉시 생성해서 반환
    public async Task<T> CreateLabel<T>(string label, string prefabName, Transform parent = null) where T : Object
    {
        var prefab = await GetLabel<T>(label, prefabName);
        return prefab != null ? Instantiate(prefab, parent) : null;

    }

    public async Task<List<T>> GetLabelAll<T>(string label) where T : Object
    {
        // 라벨이 아직 캐시에 없다면 자동으로 LoadByLabelAsync 호출
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
        // GetByLabel을 재사용
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
        task.Wait(); // Task가 끝날 때까지 대기

        return task.Result; // List<T> 반환
    }
}
