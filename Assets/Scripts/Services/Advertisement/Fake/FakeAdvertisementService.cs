using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Services.Advertisement.Fake
{
    public class FakeAdvertisementService : Service, IAdvertisementService
    {
        public UniTask ShowInterstitialAd()
        {
            Debug.Log("ShowInterstitialAd");
            return UniTask.CompletedTask;
        }

        public UniTask LoadInterstitialAd()
        {
            Debug.Log("LoadInterstitialAd");
            return UniTask.CompletedTask;
        }

        public bool HasLoadedInterstitialAd() => true;

        public UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null)
        {
            Debug.Log("ShowRewardedVideoAd");
            onRewardedCallback?.Invoke();
            return UniTask.CompletedTask;
        }

        public UniTask LoadRewardedVideoAd()
        {
            Debug.Log("LoadRewardedVideoAd");
            return UniTask.CompletedTask;
        }

        public bool HasLoadedRewardedVideoAd() => true;
    }
}