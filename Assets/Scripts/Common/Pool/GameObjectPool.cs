using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.Pool
{
    public class GameObjectPool<T> where T : MonoBehaviour, IPoolObject
    {
        private readonly List<T> _activeObjects;
        private readonly Stack<T> _unActiveObjects;

        private readonly T _template;
        private readonly Transform _parent;
        
        public GameObjectPool(T template, Transform poolParent, int cacheCount = 0)
        {
            _template = template;
            _parent = poolParent;
            
            _activeObjects = new List<T>(cacheCount);
            _unActiveObjects = new Stack<T>(cacheCount);
            for (var i = 0; i < cacheCount; i++)
            {
                var poolObject = Object.Instantiate(_template, _parent);
                _unActiveObjects.Push(poolObject);
                poolObject.gameObject.SetActive(false);
                poolObject.transform.SetParent(_parent);
            }
        }

        public T Get(Transform parent = null)
        {
            if (parent == null) parent = _parent;
            T poolObject;
            if (_unActiveObjects.Count > 0)
            {
                poolObject = _unActiveObjects.Pop();
                _activeObjects.Add(poolObject);
                poolObject.transform.SetParent(parent);
                poolObject.gameObject.SetActive(true);
                poolObject.Initialize();
                return poolObject;
            }

            poolObject = Object.Instantiate(_template, _parent);
            _activeObjects.Add(poolObject);
            poolObject.transform.SetParent(parent);
            poolObject.gameObject.SetActive(true);
            poolObject.Initialize();
            return poolObject;
        }
        
        public async UniTaskVoid ReturnAsync(T poolObject, float seconds = 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
            Return(poolObject);
        }
        
        public void Return(T poolObject)
        {
            if (!_activeObjects.Remove(poolObject))
            {
                throw new Exception("Not from this pool");
            }
            _unActiveObjects.Push(poolObject);
            poolObject.Dispose();
            poolObject.gameObject.SetActive(false);
            poolObject.transform.SetParent(_parent);
        }
    }
}