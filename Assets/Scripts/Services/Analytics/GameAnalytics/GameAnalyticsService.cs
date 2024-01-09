using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using Services.Common;
using UnityEngine;

namespace Services.Analytics.GameAnalytics
{
    public class GameAnalyticsService : Service, IAnalyticsService, IGameAnalyticsATTListener
    {
        protected override UniTask OnInitialize()
        {
            var mainGo = new GameObject {name = "GameAnalytics"};
            mainGo.AddComponent<GameAnalyticsSDK.GameAnalytics>();

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalyticsSDK.GameAnalytics.RequestTrackingAuthorization(this);
            }

            GameAnalyticsSDK.GameAnalytics.Initialize();
            return UniTask.CompletedTask;
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalyticsSDK.GameAnalytics.Initialize();
        }

        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalyticsSDK.GameAnalytics.Initialize();
        }

        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalyticsSDK.GameAnalytics.Initialize();
        }

        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalyticsSDK.GameAnalytics.Initialize();
        }

        public void SendEvent(string eventName)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName);
        }

        public void SendEvent(string eventName, Parameter parameter)
        {
            if (string.IsNullOrEmpty(parameter.Value))
            {
                var parameterValue = parameter.IntValue;
                GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName, parameterValue);
            }
            else
            {
                var fields = new Dictionary<string, object> {{parameter.Name, parameter.Value}};
                GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName, 0f, fields);
            }
        }

        public void SendEvent(string eventName, List<Parameter> parameters)
        {
            if (parameters.Count == 0)
            {
                SendEvent(eventName);
            }
            else if (parameters.Count == 1)
            {
                var parameter = parameters[0];
                SendEvent(eventName, parameter);
            }
            else
            {
                SendParametersEvent(eventName, parameters);
            }
        }

        private void SendParametersEvent(string eventName, List<Parameter> parameters)
        {
            var fields = new Dictionary<string, object>();
            var mainParameter = parameters[0];
            foreach (var parameter in parameters)
            {
                fields.Add(parameter.Name, parameter.Value);
            }

            GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName, mainParameter.IntValue, fields);
        }

        public void Flush()
        {
        }
    }
}