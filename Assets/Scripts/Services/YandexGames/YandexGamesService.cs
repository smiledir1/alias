﻿using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Services.YandexGames
{
    /// <summary>
    /// Works only in Yandex Games with WebGL
    /// </summary>
    public class YandexGamesService : Service, IYandexGamesService
    {
        [DllImport("__Internal")]
        private static extern void InitializePlayer(string playerPhotoSize, bool scopes);

        [DllImport("__Internal")]
        private static extern void FullscreenAdShow();

        [DllImport("__Internal")]
        private static extern void RewardedVideoShow();

        [DllImport("__Internal")]
        private static extern void RequestingEnvironmentData();

        [DllImport("__Internal")]
        private static extern void Review();

        [DllImport("__Internal")]
        private static extern void PromptShow();

        [DllImport("__Internal")]
        private static extern void BuyPayments(string id);

        [DllImport("__Internal")]
        private static extern void GetPayments();

        private readonly IAssetsService _assetsService;

        private YandexGamesProxy _yandexGamesProxy;

        private UnityAction _onRewardedCallback;

        private UniTaskCompletionSource<PlayerData> _initializePlayerSource;
        private UniTaskCompletionSource<bool> _fullscreenAdSource;
        private UniTaskCompletionSource<bool> _rewardedVideoAdSource;
        private UniTaskCompletionSource<EnvironmentData> _environmentDataSource;
        private UniTaskCompletionSource<ReviewReason> _reviewSource;

        public YandexGamesService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);

            _yandexGamesProxy = await _assetsService.InstantiateAsync<YandexGamesProxy>();
            _yandexGamesProxy.name = "YandexGamesProxy";

            _yandexGamesProxy.PlayerInitialized += OnPlayerInitialized;

            _yandexGamesProxy.OpenFullscreenAd += OnOpenFullscreenAd;
            _yandexGamesProxy.CloseFullscreenAd += OnCloseFullscreenAd;
            _yandexGamesProxy.ErrorFullscreenAd += OnErrorFullscreenAd;

            _yandexGamesProxy.OpenRewardedVideo += OnOpenRewardedVideo;
            _yandexGamesProxy.CloseRewardedVideo += OnCloseRewardedVideo;
            _yandexGamesProxy.RewardRewardedVideo += OnRewardRewardedVideo;
            _yandexGamesProxy.ErrorRewardedVideo += OnErrorRewardedVideo;

            _yandexGamesProxy.SetEnvironmentData += OnSetEnvironmentData;

            _yandexGamesProxy.ReviewSent += OnReviewSent;
            _yandexGamesProxy.ReviewError += OnReviewError;
        }

        protected override UniTask OnDispose()
        {
            _yandexGamesProxy.PlayerInitialized -= OnPlayerInitialized;

            _yandexGamesProxy.OpenFullscreenAd -= OnOpenFullscreenAd;
            _yandexGamesProxy.CloseFullscreenAd -= OnCloseFullscreenAd;
            _yandexGamesProxy.ErrorFullscreenAd -= OnErrorFullscreenAd;

            _yandexGamesProxy.OpenRewardedVideo -= OnOpenRewardedVideo;
            _yandexGamesProxy.CloseRewardedVideo -= OnCloseRewardedVideo;
            _yandexGamesProxy.RewardRewardedVideo -= OnRewardRewardedVideo;
            _yandexGamesProxy.ErrorRewardedVideo -= OnErrorRewardedVideo;

            _yandexGamesProxy.SetEnvironmentData -= OnSetEnvironmentData;

            _yandexGamesProxy.ReviewSent -= OnReviewSent;
            _yandexGamesProxy.ReviewError -= OnReviewError;
            
            return base.OnDispose();
        }

        #region Player

        public UniTask<PlayerData> InitializePlayer(PlayerPhotoSize photoSize)
        {
#if UNITY_EDITOR
            return UniTask.FromResult(new PlayerData());
#else
            _initializePlayerSource = new UniTaskCompletionSource<PlayerData>();
            InitializePlayer(photoSize.ToString(), true);
            return _initializePlayerSource.Task;
#endif
        }

        private void OnPlayerInitialized(string data)
        {
            var playerData = JsonUtility.FromJson<PlayerData>(data);
            _initializePlayerSource?.TrySetResult(playerData);
        }

        #endregion

        #region FullscreenAd

        public UniTask ShowFullscreenAd()
        {
#if UNITY_EDITOR
            return UniTask.FromResult(true);
#else
            _fullscreenAdSource = new UniTaskCompletionSource<bool>();
            FullscreenAdShow();
            return _fullscreenAdSource.Task;
#endif
        }

        private void OnOpenFullscreenAd()
        {
        }

        private void OnCloseFullscreenAd(bool wasShown)
        {
            _fullscreenAdSource?.TrySetResult(wasShown);
        }

        private void OnErrorFullscreenAd()
        {
            _fullscreenAdSource?.TrySetResult(false);
        }

        #endregion

        #region Rewarded Video Ad

        public UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null)
        {
#if UNITY_EDITOR
            onRewardedCallback?.Invoke();
            return UniTask.FromResult(true);
#else
            _onRewardedCallback = onRewardedCallback;
            _rewardedVideoAdSource = new UniTaskCompletionSource<bool>();
            RewardedVideoShow();
            return _rewardedVideoAdSource.Task;
#endif
        }

        public void OnOpenRewardedVideo()
        {
        }

        public void OnCloseRewardedVideo()
        {
            _rewardedVideoAdSource?.TrySetResult(true);
        }

        public void OnRewardRewardedVideo()
        {
            _onRewardedCallback?.Invoke();
            _onRewardedCallback = null;
        }

        public void OnErrorRewardedVideo()
        {
            _rewardedVideoAdSource?.TrySetResult(false);
        }

        #endregion

        #region Environment

        public UniTask<EnvironmentData> GetEnvironment()
        {
#if UNITY_EDITOR
            return UniTask.FromResult(new EnvironmentData());
#else
            _environmentDataSource = new UniTaskCompletionSource<EnvironmentData>();
            RequestingEnvironmentData();
            return _environmentDataSource.Task;
#endif
        }

        private void OnSetEnvironmentData(string data)
        {
            var environmentData = JsonUtility.FromJson<EnvironmentData>(data);
            _environmentDataSource?.TrySetResult(environmentData);
        }

        #endregion

        #region Prompt

        #endregion

        #region Review

        public UniTask<ReviewReason> ShowReview()
        {
#if UNITY_EDITOR
            return UniTask.FromResult(ReviewReason.UserRateAccepted);
#else
            _reviewSource = new UniTaskCompletionSource<ReviewReason>();
            Review();
            return _reviewSource.Task;
#endif
        }

        private void OnReviewSent(bool feedbackSent)
        {
            var reason = feedbackSent ? ReviewReason.UserRateAccepted : ReviewReason.UserRateCancelled;
            _reviewSource.TrySetResult(reason);
        }

        public void OnReviewError(string reasonType)
        {
            var reason = ReviewReasonHelper.ToEnum(reasonType);
            _reviewSource.TrySetResult(reason);
        }

        #endregion

        #region Payments

        #endregion
    }

    public class PlayerData
    {
        public string IsPlayerAuth;
        public string PlayerName;
        public string PlayerId;
        public string PlayerPhotoUrl;

        public override string ToString()
        {
            return $"EnvironmentData:" +
                   $"\nIsPlayerAuth: {IsPlayerAuth}" +
                   $"\nPlayerName: {PlayerName} " +
                   $"\nPlayerId: {PlayerId} " +
                   $"\nPlayerPhotoUrl: {PlayerPhotoUrl} ";
        }
    }

    public class EnvironmentData
    {
        public string Language = "ru";
        public string Domain = "ru";
        public string DeviceType = "desktop";
        public bool IsMobile;
        public bool IsDesktop = true;
        public bool IsTablet;
        public bool IsTV;
        public string AppID;
        public string BrowserLang;
        public string Payload;
        public bool PromptCanShow;
        public bool ReviewCanShow;

        public override string ToString()
        {
            return $"EnvironmentData:" +
                   $"\nLanguage: {Language}" +
                   $"\nDomain: {Domain} " +
                   $"\nDeviceType: {DeviceType} " +
                   $"\nIsMobile: {IsMobile} " +
                   $"\n IsDesktop: {IsDesktop}" +
                   $"\n IsTablet: {IsTablet}" +
                   $"\n IsTV: {IsTV}" +
                   $"\n AppID: {AppID}" +
                   $"\n BrowserLang: {BrowserLang}" +
                   $"\n Payload: {Payload}" +
                   $"\n PromptCanShow: {PromptCanShow}" +
                   $"\n ReviewCanShow: {ReviewCanShow}";
        }
    }

    public enum PlayerPhotoSize
    {
        Small,
        Medium,
        Large
    }

    public enum ReviewReason
    {
        NoAuth,
        GameRated,
        ReviewAlreadyRequested,
        ReviewWasRequested,
        Unknown,
        UserRateAccepted,
        UserRateCancelled
    }

    public static class PlayerPhotoSizeHelper
    {
        public static PlayerPhotoSize ToEnum(string size)
        {
            return size switch
            {
                "small" => PlayerPhotoSize.Small,
                "medium" => PlayerPhotoSize.Medium,
                "large" => PlayerPhotoSize.Large,
                _ => PlayerPhotoSize.Small,
            };
        }

        public static string ToEnumString(this PlayerPhotoSize reason)
        {
            return reason switch
            {
                PlayerPhotoSize.Small => "small",
                PlayerPhotoSize.Medium => "medium",
                PlayerPhotoSize.Large => "large",
                _ => "small"
            };
        }
    }

    public static class ReviewReasonHelper
    {
        public static ReviewReason ToEnum(string reason)
        {
            return reason switch
            {
                "NO_AUTH" => ReviewReason.NoAuth,
                "GAME_RATED" => ReviewReason.GameRated,
                "REVIEW_ALREADY_REQUESTED" => ReviewReason.ReviewAlreadyRequested,
                "REVIEW_WAS_REQUESTED" => ReviewReason.ReviewWasRequested,
                "UNKNOWN" => ReviewReason.Unknown,
                "USER_RATE_ACCEPTED" => ReviewReason.UserRateAccepted,
                "USER_RATE_CANCELLED" => ReviewReason.UserRateCancelled,
                _ => ReviewReason.Unknown,
            };
        }

        public static string ToEnumString(this ReviewReason reason)
        {
            return reason switch
            {
                ReviewReason.NoAuth => "NO_AUTH",
                ReviewReason.GameRated => "GAME_RATED",
                ReviewReason.ReviewAlreadyRequested => "REVIEW_ALREADY_REQUESTED",
                ReviewReason.ReviewWasRequested => "REVIEW_WAS_REQUESTED",
                ReviewReason.Unknown => "UNKNOWN",
                ReviewReason.UserRateAccepted => "USER_RATE_ACCEPTED",
                ReviewReason.UserRateCancelled => "USER_RATE_CANCELLED",
                _ => "UNKNOWN"
            };
        }
    }
}