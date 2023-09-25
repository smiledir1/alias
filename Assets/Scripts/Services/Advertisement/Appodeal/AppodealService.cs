using System;
using AppodealStack.Monetization.Common;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;
using UnityEngine.Events;

namespace Services.Advertisement.Appodeal
{
    public class AppodealService : Service, IAdvertisementService
    {
        private readonly AssetsService _assetsService;
        private AppodealConfig _appodealConfig;
        private UniTaskCompletionSource _interstitialSource;
        private UniTaskCompletionSource _rewardedSource;
        private UnityAction _rewardedCallback;

        public AppodealService(AssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);

            _appodealConfig = await _assetsService.LoadAsset<AppodealConfig>();

            var adTypes = AppodealAdType.None;
            if (_appodealConfig.UseInterstitial)
            {
                adTypes |= AppodealAdType.Interstitial;
                AppodealStack.Monetization.Api.Appodeal.SetAutoCache(AppodealAdType.Interstitial, false);
            }

            if (_appodealConfig.UseRewardedVideo) adTypes |= AppodealAdType.RewardedVideo;
            if (_appodealConfig.IsTesting)
            {
                AppodealStack.Monetization.Api.Appodeal.SetTesting(true);
                AppodealStack.Monetization.Api.Appodeal.SetLogLevel(AppodealLogLevel.Verbose);
            }

            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            AppodealStack.Monetization.Api.Appodeal.MuteVideosIfCallsMuted(true);
            AppodealStack.Monetization.Api.Appodeal.Initialize(_appodealConfig.AppKey, adTypes);
        }

        protected override UniTask OnDispose()
        {
            AppodealCallbacks.Sdk.OnInitialized -= OnInitializationFinished;
            return UniTask.CompletedTask;
        }

        private void OnInitializationFinished(object sender, SdkInitializedEventArgs e)
        {
            AppodealCallbacks.Interstitial.OnShown += OnInterstitialShown;
            AppodealCallbacks.Interstitial.OnShowFailed += OnInterstitialShowFailed;
            AppodealCallbacks.Interstitial.OnExpired += OnInterstitialExpired;

            AppodealCallbacks.RewardedVideo.OnShown += OnRewardedVideoShown;
            AppodealCallbacks.RewardedVideo.OnShowFailed += OnRewardedVideoShowFailed;
            AppodealCallbacks.RewardedVideo.OnFinished += OnRewardedVideoFinished;
            AppodealCallbacks.RewardedVideo.OnExpired += OnRewardedVideoExpired;

            SetStarted().Forget();
        }

        #region Interstitial

        public bool HasLoadedInterstitialAd()
        {
            if (State != ServiceState.Started) return false;
            return AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.Interstitial);
        }

        public UniTask LoadInterstitialAd()
        {
            if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.Interstitial))
                return UniTask.CompletedTask;

            if (State != ServiceState.Started) return UniTask.CompletedTask;

            AppodealStack.Monetization.Api.Appodeal.Cache(AppodealAdType.Interstitial);
            return UniTask.CompletedTask;
        }

        public async UniTask ShowInterstitialAd()
        {
            if (State != ServiceState.Started) return;

            if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.Interstitial))
            {
                _interstitialSource?.TrySetCanceled();
                _interstitialSource = new UniTaskCompletionSource();
                AppodealStack.Monetization.Api.Appodeal.Show(AppodealShowStyle.Interstitial);
                await _interstitialSource.Task;
            }
        }

        #region InterstitialAd Callbacks

        // Called when interstitial was loaded, but cannot be shown (internal network errors, placement settings, etc.)
        private void OnInterstitialShowFailed(object sender, EventArgs e)
        {
            _interstitialSource.TrySetCanceled();
        }

        // Called when interstitial is shown
        private void OnInterstitialShown(object sender, EventArgs e)
        {
            _interstitialSource.TrySetResult();
        }

        // Called when interstitial is expired and can not be shown
        private void OnInterstitialExpired(object sender, EventArgs e)
        {
            _interstitialSource.TrySetCanceled();
        }

        #endregion

        #endregion

        #region RewardedVideo

        public bool HasLoadedRewardedVideoAd()
        {
            if (State != ServiceState.Started) return false;
            return AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.RewardedVideo);
        }

        public UniTask LoadRewardedVideoAd()
        {
            if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
                return UniTask.CompletedTask;

            if (State == ServiceState.Started)
            {
                AppodealStack.Monetization.Api.Appodeal.Cache(AppodealAdType.RewardedVideo);
            }

            return UniTask.CompletedTask;
        }

        public async UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null)
        {
            if (State != ServiceState.Started) return;

            if (AppodealStack.Monetization.Api.Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
            {
                _rewardedCallback = onRewardedCallback;
                _rewardedSource?.TrySetCanceled();
                _rewardedSource = new UniTaskCompletionSource();
                AppodealStack.Monetization.Api.Appodeal.Show(AppodealShowStyle.RewardedVideo);
                await _rewardedSource.Task;
            }
        }

        #region RewardedVideo Callbacks

        // Called when interstitial was loaded, but cannot be shown (internal network errors, placement settings, etc.)
        private void OnRewardedVideoShowFailed(object sender, EventArgs e)
        {
            _rewardedSource.TrySetCanceled();
        }

        // Called when interstitial is shown
        private void OnRewardedVideoShown(object sender, EventArgs e)
        {
            _rewardedSource.TrySetResult();
        }

        // Called when interstitial is expired and can not be shown
        private void OnRewardedVideoExpired(object sender, EventArgs e)
        {
            _rewardedSource.TrySetCanceled();
        }

        // Called when rewarded video is viewed until the end
        private void OnRewardedVideoFinished(object sender, RewardedVideoFinishedEventArgs e)
        {
            OnGetReward().Forget();
        }

        private async UniTask OnGetReward()
        {
            await UniTask.SwitchToMainThread();
            _rewardedCallback?.Invoke();
        }

        #endregion

        #endregion
    }
}