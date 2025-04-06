using System;
using System.Collections.Generic;
using UnityEngine;

namespace C2Project.Core
{
    public class SpriteLoader : IDisposable
    {
        private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>(); // 스프라이트 캐시

        public void AddSprite(string name, Sprite sprite)
        {
            if (_spriteCache.ContainsKey(name))
            {
                Debug.LogWarning($"Sprite with name {name} already exists in cache. Overwriting.");
            }

            _spriteCache[name] = sprite;
        }

        public Sprite GetSprite(string name)
        {
            if (!_spriteCache.ContainsKey(name))
                return null;

            return _spriteCache[name];
        }

        public void Dispose()
        {
            _spriteCache.Clear();
        }

    }
}