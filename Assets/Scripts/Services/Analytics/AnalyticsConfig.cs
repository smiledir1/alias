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
        private bool _unityAnalytics;

        public bool IsUnityAnalytics => _unityAnalytics;


#if UNITY_EDITOR

        private const string UnityAnalyticsDefine = "ANALYTICS_UNITY";

        private void OnValidate()
        {
            UnityEditor.PlayerSettings.GetScriptingDefineSymbols(
                UnityEditor.Build.NamedBuildTarget.Standalone,
                out var projectDefines);
            var addDefines = new List<string>(projectDefines);
            
            if (_unityAnalytics)
            {
                addDefines.Add(UnityAnalyticsDefine);
            }
            else
            {
                addDefines.Remove(UnityAnalyticsDefine);
            }

            DefinesUtils.SetDefinesForTargets(addDefines);
        }
#endif
    }
}