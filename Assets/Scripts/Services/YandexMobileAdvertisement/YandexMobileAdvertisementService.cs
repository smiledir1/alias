using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.Advertisement;
using Services.Common;
using UnityEngine;
using UnityEngine.Events;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Services.YandexMobileAdvertisement
{
    public class YandexMobileAdvertisementService : Service, IAdvertisementService
    {
        private readonly InterstitialAdLoader _interstitialAdLoader;
        private readonly Dictionary<string, Interstitial> _idToInterstitial = new();

        public YandexMobileAdvertisementService()
        {
            _interstitialAdLoader = new InterstitialAdLoader();
            _interstitialAdLoader.OnAdLoaded += HandleInterstitialLoaded;
            _interstitialAdLoader.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
        }

        #region Interstitial Methods

        public void LoadInterstitial(string id)
        {
           if (_idToInterstitial.ContainsKey(id)) return;
            _idToInterstitial.Add(id, null);
            
            // замените на "R-M-XXXXXX-Y"
            //var adUnitId = "demo-interstitial-yandex";
            //var adUnitId = "R-M-2954019-1";
            var adRequestConfiguration = new AdRequestConfiguration.Builder(id).Build();
            _interstitialAdLoader.LoadAd(adRequestConfiguration);
        }

        public bool HasLoadedInterstitial(string id) => GetInterstitial(id) != null;

        public bool ShowInterstitial(string id)
        {
            var interstitial = GetInterstitial(id);
            if (interstitial == null) return false;
            interstitial.Show();
            return true;
        }

        private Interstitial GetInterstitial(string id) =>
            !_idToInterstitial.TryGetValue(id, out var interstitial) 
                ? null 
                : interstitial;

        private void HandleInterstitialLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            var interstitial = args.Interstitial;
            var adKey = interstitial.GetInfo().AdUnitId;
            if (!_idToInterstitial.ContainsKey(adKey))
            {
                _idToInterstitial.Add(adKey, interstitial);
            }
            else
            {
                _idToInterstitial[adKey] = interstitial;
            }
            interstitial.OnAdClicked += HandleAdClicked;
            interstitial.OnAdShown += HandleInterstitialShown;
            interstitial.OnAdFailedToShow += HandleInterstitialFailedToShow;
            interstitial.OnAdImpression += HandleImpression;
            interstitial.OnAdDismissed += HandleInterstitialDismissed;
        }

        private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogError($"HandleInterstitialFailedToLoad {args.Message} {args.AdUnitId}");
            _idToInterstitial.Remove(args.AdUnitId);
            // Ad {args.AdUnitId} failed for to load with {args.Message}
            // Attempting to load a new ad from the OnAdFailedToLoad event is strongly discouraged.
        }

        private void HandleInterstitialDismissed(object sender, EventArgs args)
        {
            // Called when ad is dismissed.
            var interstitial = sender as Interstitial;
            Debug.Log(interstitial?.GetInfo().AdUnitId);
            // Clear resources after Ad dismissed.
            DestroyInterstitial(interstitial);

            // Now you can preload the next interstitial ad.
            //RequestInterstitial();
        }

        private void HandleInterstitialFailedToShow(object sender, EventArgs args)
        {
            Debug.LogError($"HandleInterstitialFailedToShow ");
            // Called when an InterstitialAd failed to show.

            // Clear resources after Ad dismissed.
            DestroyInterstitial(sender as Interstitial);
            // Now you can preload the next interstitial ad.
            //RequestInterstitial();
        }

        private void HandleAdClicked(object sender, EventArgs args)
        {
            Debug.LogError($"HandleAdClicked ");
            // Called when a click is recorded for an ad.
        }

        private void HandleInterstitialShown(object sender, EventArgs args)
        {
            var interstitial = sender as Interstitial;
            Debug.Log($"{interstitial.GetInfo().AdUnitId}");
            Debug.LogError($"HandleInterstitialShown ");
            // Called when ad is shown.
        }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            Debug.LogError($"HandleImpression ");
            // Called when an impression is recorded for an ad.
        }

        private void DestroyInterstitial(Interstitial interstitial)
        {
            interstitial?.Destroy();
        }

        #endregion

        public UniTask ShowInterstitialAd()
        {
            return UniTask.CompletedTask;
        }

        public UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null)
        {
            return UniTask.CompletedTask;
        }
    }
}