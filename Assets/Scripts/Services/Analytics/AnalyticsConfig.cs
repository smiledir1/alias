using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Common.Utils.Defines;
#endif

namespace Services.Analytics
{
    [CreateAssetMenu(menuName = "Services/Analytics/AnalyticsConfig", fileName = "AnalyticsConfig")]
    public class AnalyticsConfig : ScriptableObject
    {
        [SerializeField]
        private bool unityAnalytics;

        public bool IsUnityAnalytics => unityAnalytics;


#if UNITY_EDITOR

        private const string UnityAnalyticsDefine = "ANALYTICS_UNITY";
        private bool _prevUnityAnalytics;

        private void OnEnable()
        {
            _prevUnityAnalytics = unityAnalytics;
        }

        private void OnValidate()
        {
            if (_prevUnityAnalytics == unityAnalytics) return;
            _prevUnityAnalytics = unityAnalytics;

            if (unityAnalytics)
                DefinesUtils.AddDefinesForDefaultTargets(new List<string> {UnityAnalyticsDefine});
            else
                DefinesUtils.RemoveDefinesForDefaultTargets(new List<string> {UnityAnalyticsDefine});
        }
#endif
    }
}