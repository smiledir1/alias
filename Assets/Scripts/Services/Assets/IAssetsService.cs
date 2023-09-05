using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Assets
{
    public interface IAssetsService : IService
    {
        UniTask<T> InstantiateAsync<T>() where T : MonoBehaviour;

        UniTask<T> InstantiateAsync<T>(
            Vector3 position,
            Quaternion rotation,
            Transform parent) where T : MonoBehaviour;

        UniTask<T> InstantiateAsync<T>(
            Transform parent) where T : MonoBehaviour;

        UniTask<GameObject> InstantiateAsync(string typeKey);
        UniTask<T> LoadAsset<T>() where T : class;
        bool TryGetAssetReference<T>(out AssetReference reference) where T : class;
    }
}