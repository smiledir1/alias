using System.Collections.Generic;
using Services.Common;

namespace Services.Analytics
{
    public interface IAnalyticsService : IService
    {
        void SendEvent(string eventName);
        void SendEvent(string eventName, Parameter parameter);
        void SendEvent(string eventName, List<Parameter> parameters);
        void Flush();
    }
}