using UnityEngine;

namespace Services.Advertisement.Appodeal
{
    [CreateAssetMenu(menuName = "Services/Appodeal/AppodealConfig", fileName = "AppodealConfig")]
    public class AppodealConfig : ScriptableObject
    {
        [SerializeField]
        private string appKey;

        [SerializeField]
        private bool isTesting;
        
        [SerializeField]
        private bool useInterstitial;

        [SerializeField]
        private bool useRewardedVideo;

        public string AppKey => appKey;
        public bool UseInterstitial => useInterstitial;
        public bool UseRewardedVideo => useRewardedVideo;
        public bool IsTesting => isTesting;
    }
}