using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// Addressables을 활용한 리소스 관리 서비스
public class AddressableService
{
    protected bool _init = false; 

    private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>(); // 프리팹 캐시

    /// 특정 라벨을 가진 모든 프리팹을 비동기 로드 후 캐싱
    /// <param name="label">Addressables 라벨</param>
    /// <param name="callback">로드 진행 상황 콜백 (키, 현재 로드된 개수, 총 개수)</param>
    public async Task LoadAllPrefabsAsync(string label, Action<string, int, int> callback = null)
    {
        if (_init)
        {
            Debug.Log("[AddressableService] 프리팹이 이미 초기화되었습니다.");
            return;
        }

        // 지정된 라벨을 가진 모든 프리팹의 위치 정보를 로드
        var locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(GameObject));
        await locationsHandle.Task;

        if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[AddressableService] 리소스 위치 로드 실패: {locationsHandle.Status}");
            return;
        }

        int loadCount = 0;
        int totalCount = locationsHandle.Result.Count;

        foreach (var location in locationsHandle.Result)
        {
            string cleanKey = GetCleanKey(location.PrimaryKey); // 경로 정리
            var prefabHandle = Addressables.LoadAssetAsync<GameObject>(location.PrimaryKey);
            await prefabHandle.Task;

            if (prefabHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if(location.PrimaryKey.Contains(".sprite"))
                {
                    // 스프라이트는 따로 예외처리
                }
                else
                {
                    _prefabCache[cleanKey] = prefabHandle.Result; // 정리된 키로 저장
                    loadCount++;
                    callback?.Invoke(location.PrimaryKey, loadCount, totalCount);
                }
            }
            else
            {
                Debug.LogError($"[AddressableService] 프리팹 로드 실패: {location.PrimaryKey}");
            }
        }

        _init = true;
        Debug.Log("[AddressableService] 모든 프리팹이 로드 및 캐싱되었습니다.");
    }

    /// 로드된 프리팹을 캐싱된 딕셔너리에서 가져오기
    public GameObject GetPrefab(string key)
    {
        if (_prefabCache.TryGetValue(key, out var prefab))
        {
            return prefab;
        }

        Debug.LogError($"[AddressableService] 캐싱된 프리팹을 찾을 수 없음: {key}");
        return null;
    }

    /// Addressables에서 로드된 프리팹을 인스턴스화 (풀링 적용 가능)
    public GameObject Instantiate(string key, Transform parent = null, bool usePooling = false)
    {
        GameObject prefab = GetPrefab(key);
        if (prefab == null) return null;

        // 오브젝트 풀링을 사용할 경우 풀에서 가져오기 (현재 주석 처리됨)
        // if (usePooling)
        //     return ObjectPool.Instance.Pop(prefab);

        return GameObject.Instantiate(prefab, parent);
    }

    /// Addressables 리소스를 해제하여 메모리 정리
    public void ReleaseAll()
    {
        foreach (var key in _prefabCache.Keys)
            Addressables.Release(_prefabCache[key]);

        _prefabCache.Clear();
        _init = false;
        Debug.Log("[AddressableService] 모든 캐싱된 프리팹이 해제되었습니다.");
    }

    /// Addressables의 Primary Key에서 불필요한 경로 제거하여 프리팹 이름만 반환
    private string GetCleanKey(string fullPath)
    {
        return System.IO.Path.GetFileNameWithoutExtension(fullPath); // 확장자 제거 후 파일명 반환
    }
}
