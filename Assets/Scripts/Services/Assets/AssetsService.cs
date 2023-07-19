using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Assets
{
    public class AssetsService : Service, IAssetsService
    {
        #region Service

        protected override async UniTask OnInitialize()
        {
            await Addressables.InitializeAsync().ToUniTask();
        }

        #endregion

        #region IAssets

        public async UniTask<T> InstantiateAsync<T>() where T : MonoBehaviour
        {
            var assetReference = GetAssetReference<T>();
            var go = await Addressables.InstantiateAsync(assetReference);
            return go.GetComponent<T>();
        }

        public async UniTask<T> InstantiateAsync<T>(
            Vector3 position,
            Quaternion rotation,
            Transform parent) where T : MonoBehaviour
        {
            var assetReference = GetAssetReference<T>();
            var go = await Addressables.InstantiateAsync(
                assetReference, position, rotation, parent);
            return go.GetComponent<T>();
        }

        public async UniTask<T> InstantiateAsync<T>(
            Transform parent) where T : MonoBehaviour
        {
            var assetReference = GetAssetReference<T>();
            var go = await Addressables.InstantiateAsync(
                assetReference, parent);
            return go.GetComponent<T>();
        }

        public async UniTask<GameObject> InstantiateAsync(string typeKey)
        {
            var assetReference = GetAssetReference(typeKey);
            return await Addressables.InstantiateAsync(assetReference);
        }

        public async UniTask<T> LoadAsset<T>() where T : class
        {
            var assetReference = GetAssetReference<T>();
            return await assetReference.LoadAssetAsync<T>();
        }

        public bool TryGetAssetReference<T>(out AssetReference reference) where T : class
        {
            reference = GetAssetReference<T>();
            return reference != null;
        }

        #endregion

        #region Private

        private AssetReference GetAssetReference(string typeKey)
        {
            foreach (var item in Addressables.ResourceLocators)
            {
                foreach (var key in item.Keys)
                {
                    if (key is not string keyStr) continue;
                    if (keyStr.Length == typeKey.Length)
                    {
                        if (keyStr != typeKey)
                            continue;
                    }
                    else if (!keyStr.EndsWith($"/{typeKey}"))
                        continue;

                    return new AssetReference(keyStr);
                }
            }

            return null;
        }

        private AssetReference GetAssetReference<T>() where T : class
        {
            var typeKey = typeof(T).Name;
            return GetAssetReference(typeKey);
        }

        #endregion
    }
}