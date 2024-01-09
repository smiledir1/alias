using UnityEngine;

namespace Services.Advertisement.Appodeal
{
    [CreateAssetMenu(menuName = "Services/Appodeal/AppodealConfig", fileName = "AppodealConfig")]
    public class AppodealConfig : ScriptableObject
    {
        [SerializeField]
        private string appKeyAndroid;

        [SerializeField]
        private string appKeyIos;

        [SerializeField]
        private bool isTesting;
        
        [SerializeField]
        private bool useInterstitial;

        [SerializeField]
        private bool useRewardedVideo;

        
        public string AppKeyAndroid => appKeyAndroid;
        public string AppKeyIos => appKeyIos;
        public bool UseInterstitial => useInterstitial;
        public bool UseRewardedVideo => useRewardedVideo;
        public bool IsTesting => isTesting;
    }
}