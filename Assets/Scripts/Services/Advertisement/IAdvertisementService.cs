using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine.Events;

namespace Services.Advertisement
{
    public interface IAdvertisementService : IService
    {
        UniTask ShowInterstitialAd();
        UniTask LoadInterstitialAd();
        bool HasLoadedInterstitialAd();
        UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null);
        UniTask LoadRewardedVideoAd();
        bool HasLoadedRewardedVideoAd();
    }
}