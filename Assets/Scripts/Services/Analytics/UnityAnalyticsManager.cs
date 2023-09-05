#if ANALYTICS_UNITY
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;

namespace Services.Analytics
{
    public class UnityAnalyticsManager : IAnalyticsManager
    {
        private Unity.Services.Analytics.IAnalyticsService _analytics;

        public async UniTask Initialize()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized) await UnityServices.InitializeAsync();

            _analytics = Unity.Services.Analytics.AnalyticsService.Instance;
            _analytics.StartDataCollection();
        }

        public void SendEvent(string eventName)
        {
            _analytics.CustomData(eventName);
        }

        public void SendEvent(string eventName, List<Parameter> parameters)
        {
            var currentParams = new Dictionary<string, object>();
            foreach (var parameter in parameters)
            {
                currentParams.Add(parameter.Name, parameter.Value);
            }

            _analytics.CustomData(eventName, currentParams);
        }

        public void Flush()
        {
            _analytics.Flush();
        }
    }
}
#endif