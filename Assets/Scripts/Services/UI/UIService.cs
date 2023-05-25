using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services.UI
{
    public abstract class UIService : Service, IUIService
    {
        #region Cache

        private int _rayCastCount;

        protected readonly IAssetsService AssetsService;
        protected readonly Dictionary<Type, UIObject> UIObjectsLoadCache = new();
        protected readonly List<UIObject> ShowedListUIObjects = new();

        #endregion

        public UIService(IAssetsService assetsService)
        {
            AssetsService = assetsService;
        }

        protected abstract UICanvas Canvas { get; }

        #region IUIService

        public event Action<UIObject> UIObjectOpen;
        public event Action<UIObject> UIObjectClose;
        public event Action<UIObject> UIObjectStart;
        public event Action<UIObject> UIObjectStop;
        public event Action<UIObject> UIObjectLoad;
        public event Action<UIObject> UIObjectUnload;

        public virtual async UniTask<T> ShowAsync<T>(
            UIModel uiModel,
            bool closePreviousUI = false,
            int group = default) where T : UIObject
        {
            IncrementBlockRaycast();
            try
            {
                var uiObject = await LoadUIObject<T>();

                if (ShowedListUIObjects.Count > 0)
                {
                    if (ShowedListUIObjects.Contains(uiObject))
                    {
                        Debug.LogWarning("Popup Already Opened");
                        return uiObject;
                    }

                    var currentUIObject = ShowedListUIObjects[^1];
                    if (closePreviousUI)
                    {
                        await CloseUIObject(currentUIObject);
                    }
                    else
                    {
                        await StopUIObject(currentUIObject);
                    }
                }

                AddShowedObjectToList(uiObject);
                uiObject.Initialize(uiModel, group);

                await OpenUIObject(uiObject);
                return uiObject;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            finally
            {
                DecrementBlockRaycast();
            }
        }

        public async UniTask CloseCurrentUIObject()
        {
            if (ShowedListUIObjects.Count == 0) return;

            IncrementBlockRaycast();

            var lastUIObject = ShowedListUIObjects[^1];
            await CloseUIObject(lastUIObject);
            RemoveShowedObjectToList(lastUIObject);

            if (ShowedListUIObjects.Count > 0)
            {
                var currentUIObject = ShowedListUIObjects[^1];
                await OpenUIObject(currentUIObject);
            }

            DecrementBlockRaycast();
        }

        public async UniTask CloseCurrentUIObject<T>() where T : UIObject
        {
            if (!UIObjectsLoadCache.TryGetValue(typeof(T), out var uiObject)) return;
            await CloseCurrentUIObject(uiObject);
        }

        public async UniTask CloseCurrentUIObject(UIObject uiObject)
        {
            var uiObjectIndex = ShowedListUIObjects.IndexOf(uiObject);
            if (uiObjectIndex == -1) return;

            IncrementBlockRaycast();

            var showedUIObjectsCountLastIndex = ShowedListUIObjects.Count - 1;
            await CloseUIObject(uiObject);
            RemoveShowedObjectToList(uiObject);

            if (showedUIObjectsCountLastIndex > 0 &&
                uiObjectIndex == showedUIObjectsCountLastIndex)
            {
                var currentUIObject = ShowedListUIObjects[^1];
                await OpenUIObject(currentUIObject);
            }

            DecrementBlockRaycast();
        }

        public async UniTask CloseGroupUIObjects(int group)
        {
            var closedUIObjectsTasks = new List<UniTask>();
            foreach (var showedUIObject in ShowedListUIObjects)
            {
                if (showedUIObject.Group != group) continue;
                closedUIObjectsTasks.Add(CloseUIObject(showedUIObject));
            }

            await UniTask.WhenAll(closedUIObjectsTasks);
        }

        public async UniTask UnloadAll()
        {
            var unloadTasks = new List<UniTask>();
            foreach (var cache in UIObjectsLoadCache.Values)
            {
                unloadTasks.Add(UnloadUIObject(cache, false));
            }

            UIObjectsLoadCache.Clear();
            ShowedListUIObjects.Clear();
            ClearBlockRaycast();
            await UniTask.WhenAll(unloadTasks);
        }

        public async UniTask<T> LoadUIObject<T>() where T : UIObject
        {
            if (UIObjectsLoadCache.TryGetValue(typeof(T), out var tUIObject))
            {
                return (T) tUIObject;
            }

            if (!AssetsService.TryGetAssetReference<T>(out var reference))
            {
                throw new NullReferenceException($"IUIObject {typeof(T).Name} not found");
            }

            var parentTransform = Canvas ? Canvas.transform : null;
            var uiGameObject = await reference.InstantiateAsync(parentTransform);
            uiGameObject.SetActive(false);
            if (!uiGameObject.TryGetComponent<T>(out var uiObject))
                throw new NullReferenceException($"No component of type {typeof(T).Name} on UI!");

            UIObjectsLoadCache.Add(typeof(T), uiObject);
            UIObjectLoad?.Invoke(uiObject);
            await uiObject.LoadAsync();

            uiObject.CloseAction += () => CloseCurrentUIObject(uiObject).Forget();

            return uiObject;
        }

        public async UniTask UnLoadUIObject<T>() where T : UIObject
        {
            if (!UIObjectsLoadCache.TryGetValue(typeof(T), out var tUIObject))
            {
                Debug.LogError($"Trying unload not cached ui object of {typeof(T).Name}");
                return;
            }

            UIObjectsLoadCache.Remove(typeof(T));
            await UnloadUIObject(tUIObject, false);
        }

        #endregion

        #region Protected Methods

        protected async UniTask OpenUIObject(UIObject uiObject)
        {
            IncrementBlockRaycast();
            try
            {
                uiObject.transform.SetAsLastSibling();
                uiObject.gameObject.SetActive(true);
                if (uiObject.State == UIObjectState.Loaded)
                {
                    UIObjectOpen?.Invoke(uiObject);
                    await uiObject.OpenAsync();
                }

                UIObjectStart?.Invoke(uiObject);
                await uiObject.StartAsync();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                DecrementBlockRaycast();
            }
        }

        protected async UniTask StopUIObject(UIObject uiObject)
        {
            if (uiObject.State == UIObjectState.Started)
            {
                UIObjectStop?.Invoke(uiObject);
                await uiObject.StopAsync();
            }
        }

        protected async UniTask CloseUIObject(UIObject uiObject)
        {
            IncrementBlockRaycast();

            if (uiObject.State == UIObjectState.Started)
            {
                UIObjectStop?.Invoke(uiObject);
                await uiObject.StopAsync();
            }

            UIObjectClose?.Invoke(uiObject);
            await uiObject.CloseAsync();
            uiObject.gameObject.SetActive(false);

            DecrementBlockRaycast();
        }

        protected async UniTask UnloadUIObject(UIObject uiObject, bool clearFromCache = true)
        {
            if (uiObject.State == UIObjectState.Started)
            {
                UIObjectStop?.Invoke(uiObject);
                await uiObject.StopAsync();
            }

            if (uiObject.State == UIObjectState.Opened)
            {
                UIObjectClose?.Invoke(uiObject);
                await uiObject.CloseAsync();
            }

            if (clearFromCache)
            {
                var keyValuePair = UIObjectsLoadCache.First(
                    o => o.Value == uiObject);
                UIObjectsLoadCache.Remove(keyValuePair.Key);
            }

            UIObjectUnload?.Invoke(uiObject);
            await uiObject.UnLoadAsync();
            Object.Destroy(uiObject.gameObject);
        }

        #endregion

        #region Private Method

        private void IncrementBlockRaycast()
        {
            _rayCastCount++;
            if (_rayCastCount > 0) Canvas.EnableRaycast();
        }

        private void DecrementBlockRaycast()
        {
            _rayCastCount--;
            if (_rayCastCount < 0)
            {
                Debug.LogWarning("DecrementBlockRaycast < 0");
                _rayCastCount = 0;
            }

            if (_rayCastCount == 0) Canvas.DisableRaycast();
        }

        private void ClearBlockRaycast()
        {
            _rayCastCount = 0;
            Canvas.DisableRaycast();
        }

        private void AddShowedObjectToList(UIObject uiObject)
        {
            IncrementBlockRaycast();
            ShowedListUIObjects.Add(uiObject);
        }

        private void RemoveShowedObjectToList(UIObject uiObject)
        {
            DecrementBlockRaycast();
            ShowedListUIObjects.Remove(uiObject);
        }

        #endregion
    }
}