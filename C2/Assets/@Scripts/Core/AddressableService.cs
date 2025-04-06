using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RSG;
using UnityEngine.U2D;
using Zenject;
using C2Project.Core;


namespace C2Project.Addressable
{
    /// Addressables을 활용한 리소스 관리 서비스
    public class AddressableService : IDisposable
    {
        [Inject] private SpriteLoader _spriteLoader;
        [Inject] private ResourceLoader _resourceLoader;
        private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>(); // 프리팹 캐시


        #region Load Reource
        /// 특정 라벨을 가진 모든 프리팹을 비동기 로드 후 캐싱
        /// <param name="label">Addressables 라벨</param>
        /// <param name="callback">로드 진행 상황 콜백 (키, 현재 로드된 개수, 총 개수)</param>
        public IPromise LoadAllPrefabsAsync<T>(string label)
        {
            var promise = new Promise();

            // 지정된 라벨을 가진 모든 오브젝트의 위치 정보를 로드
            Addressables.LoadResourceLocationsAsync(label, typeof(T))
            .Completed += locationHandle =>
            {
                if (locationHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"[AddressableService] 리소스 위치 로드 실패: {locationHandle.Status}");
                    promise.Reject(new Exception("리소스 위치 로드 실패"));
                    return;
                }

                int loadCount = 0;
                int totalCount = locationHandle.Result.Count;

                
                foreach (var location in locationHandle.Result)
                {
                    string cleanKey = GetCleanKey(location.PrimaryKey); // 경로 정리              
                    Addressables.LoadAssetAsync<T>(location.PrimaryKey)
                        .Completed += prefabHandle =>
                    {
                        if (prefabHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            var type = prefabHandle.Result;
                            if (type is GameObject gameObject)
                            {
                                LoadGameObject(cleanKey, gameObject, ref loadCount); // 프리팹 로드
                            }
                            else if (type is SpriteAtlas spriteAtlas)
                            {
                                LoadSpriteAtlas(spriteAtlas, ref loadCount); // 스프라이트 아틀라스 로드
                            }
                            else
                            {
                                Debug.LogError($"[AddressableService] 잘못된 타입: {type.GetType()}");
                            }





                            // 모든 작업이 완료되었는지 확인
                            if (loadCount == totalCount)
                            {
                                Debug.Log("[AddressableService] 모든 프리팹이 로드 및 캐싱되었습니다.");
                                promise.Resolve(); // 전체 작업 성공
                            }
                            else
                            {
                                promise.ReportProgress((float)loadCount / totalCount); // 진행 상황 보고
                            }
                        }
                        else
                        {
                            Debug.LogError($"[AddressableService] 프리팹 로드 실패: {location.PrimaryKey}");
                            promise.Reject(new Exception($"프리팹 로드 실패: {location.PrimaryKey}"));
                        }
                    };
                }
            };

            return promise;
        }

        private void LoadGameObject(string name, GameObject gameObject, ref int loadCount)
        {
            if(gameObject == null) 
            {
                Debug.LogError($"[AddressableService] 로드된 프리팹이 null입니다: {name}");
                return;
            }

            _resourceLoader.AddGameObject(name, gameObject);
            loadCount++;
        }

        private void LoadSpriteAtlas(SpriteAtlas spriteAtlas, ref int loadCount)
        {
            if (spriteAtlas == null)
            {
                Debug.LogError($"[AddressableService] 로드된 스프라이트 아틀라스가 null입니다.");
                return;
            }


            Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
            spriteAtlas.GetSprites(sprites); // 스프라이트 배열로 가져오기
            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    string cleanName = sprite.name.Split('_')[0].Replace("(Clone)", "").Trim();
                    _spriteLoader.AddSprite(cleanName, sprite); 
                }

            }
            loadCount++;
        }

        #endregion

        /// Addressables의 Primary Key에서 불필요한 경로 제거하여 프리팹 이름만 반환
        private string GetCleanKey(string fullPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(fullPath); // 확장자 제거 후 파일명 반환
        }

        public void Dispose()
        {
            foreach (var key in _prefabCache.Keys)
                Addressables.Release(_prefabCache[key]);

        }
    }
}