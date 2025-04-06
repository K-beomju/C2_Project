using System;
using System.Collections.Generic;
using UnityEngine;

namespace C2Project.Core
{
    public class ResourceLoader : IDisposable
    {
        private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>(); // 프리팹 캐시

        public void AddGameObject(string name, GameObject gameObject)
        {
            if (_prefabCache.ContainsKey(name))
            {
                Debug.LogWarning($"Sprite with name {name} already exists in cache. Overwriting.");
            }

            _prefabCache[name] = gameObject;
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


        public void Dispose()
        {
            _prefabCache.Clear();
        }


    }
}
