using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Services.Analytics
{
    public interface IAnalyticsManager
    {
        UniTask Initialize();
        void SendEvent(string eventName);
        void SendEvent(string eventName, List<Parameter> parameters);
        void Flush();
    }
}