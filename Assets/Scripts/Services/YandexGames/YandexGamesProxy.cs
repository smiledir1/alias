using System;
using UnityEngine;

namespace Services.YandexGames
{
    public class YandexGamesProxy : MonoBehaviour
    {
        public event Action<string> PlayerInitialized;
        public event Action OpenFullscreenAd;
        public event Action<bool> CloseFullscreenAd;
        public event Action ErrorFullscreenAd;
        public event Action OpenRewardedVideo;
        public event Action CloseRewardedVideo;
        public event Action RewardRewardedVideo;
        public event Action ErrorRewardedVideo;
        public event Action<string> PurchaseSuccess;
        public event Action<string> PurchaseFailed;
        public event Action<string> PaymentsEntries;
        public event Action<string> SetEnvironmentData;
        public event Action<bool> ReviewSent;
        public event Action<string> ReviewError;
        public event Action PromptSuccess;
        public event Action PromptFail;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        
        #region Player

        public void OnPlayerInitialized(string data)
        {
            PlayerInitialized?.Invoke(data);
        }

        #endregion

        #region FullscreenAd

        public void OnOpenFullscreenAd()
        {
            OpenFullscreenAd?.Invoke();
        }

        public void OnCloseFullscreenAd(int wasShownValue)
        {
            var wasShown = wasShownValue == 1;
            CloseFullscreenAd?.Invoke(wasShown);
        }

        public void OnErrorFullscreenAd()
        {
            ErrorFullscreenAd?.Invoke();
        }

        #endregion

        #region Rewarded Video

        public void OnOpenRewardedVideo()
        {
            OpenRewardedVideo?.Invoke();
        }

        public void OnCloseRewardedVideo()
        {
            CloseRewardedVideo?.Invoke();
        }

        public void OnRewardRewardedVideo()
        {
            RewardRewardedVideo?.Invoke();
        }

        public void OnErrorRewardedVideo()
        {
            ErrorRewardedVideo?.Invoke();
        }

        #endregion

        #region Payments

        public void OnPurchaseSuccess(string id)
        {
            PurchaseSuccess?.Invoke(id);
        }

        public void OnPurchaseFailed(string id)
        {
            PurchaseFailed?.Invoke(id);
        }

        public void OnPaymentsEntries(string data)
        {
            PaymentsEntries?.Invoke(data);
            var paymentsData = JsonUtility.FromJson<JsonPayments>(data);
        }

        #endregion

        #region Environment

        public void OnSetEnvironmentData(string data)
        { 
            SetEnvironmentData?.Invoke(data);
        }

        #endregion

        #region Review

        public void OnReviewSent2(string feedbackSent)
        {
            Debug.Log($"OnReviewSent2 {feedbackSent}");
        }

        public void OnReviewSent(int feedbackSent)
        {
            var feedbackSentBool = feedbackSent == 1;
            Debug.Log($"OnReviewSent1 {feedbackSentBool}");
            ReviewSent?.Invoke(feedbackSentBool);
        }
        
        public void OnReviewError(string reasonType)
        {
            ReviewError?.Invoke(reasonType);
        }

        #endregion

        #region Prompt

        public void OnPromptSuccess()
        {
            PromptSuccess?.Invoke();
        }

        public void OnPromptFail()
        {
            PromptFail?.Invoke();
        }

        #endregion

        #region Saves

        #endregion
    }

    public class JsonPayments
    {
        public string[] id;
        public string[] title;
        public string[] description;
        public string[] imageURI;
        public string[] priceValue;
        public int[] purchased;
    }
}