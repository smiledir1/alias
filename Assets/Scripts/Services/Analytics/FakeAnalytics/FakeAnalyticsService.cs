using System.Collections.Generic;
using Services.Common;
using UnityEngine;

namespace Services.Analytics.FakeAnalytics
{
    public class FakeAnalyticsService : Service, IAnalyticsService
    {
        public void SendEvent(string eventName)
        {
            Debug.Log($"Send Event: {eventName}");
        }

        public void SendEvent(string eventName, List<Parameter> parameters)
        {
            var eventParameters = string.Empty;
            foreach (var parameter in parameters)
            {
                eventParameters += $"{parameter.Name} {parameter.Value}\n";
            }

            Debug.Log($"Send Event: {eventName}\n {eventParameters}");
        }

        public void Flush()
        {
            Debug.Log($"Do Events Flush");
        }
    }
}