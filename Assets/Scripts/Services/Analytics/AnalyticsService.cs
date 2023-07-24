using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;

namespace Services.Analytics
{
    public class UniversalAnalyticsService : Service, IAnalyticsService
    {
        private readonly IAssetsService _assetsService;

        private AnalyticsConfig _analyticsConfig;
        private List<IAnalyticsManager> _managers = new();

        public UniversalAnalyticsService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);

            _analyticsConfig = await _assetsService.LoadAsset<AnalyticsConfig>();

            _managers = new List<IAnalyticsManager>();

            if (_analyticsConfig.IsUnityAnalytics) _managers.Add(CreateUnityAnalytics());

            var initializeTasks = new List<UniTask>();
            foreach (var analyticsManager in _managers)
            {
                initializeTasks.Add(analyticsManager.Initialize());
            }

            await UniTask.WhenAll(initializeTasks);
        }

        public void SendEvent(string eventName)
        {
            foreach (var manager in _managers)
            {
                manager.SendEvent(eventName);
            }
        }

        public void SendEvent(string eventName, List<Parameter> parameters)
        {
            foreach (var manager in _managers)
            {
                manager.SendEvent(eventName, parameters);
            }
        }

        public void Flush()
        {
            foreach (var manager in _managers)
            {
                manager.Flush();
            }
        }

        private IAnalyticsManager CreateUnityAnalytics()
        {
#if ANALYTICS_UNITY
            return new UnityAnalyticsManager();
#else
            return null;
#endif
        }
    }
}