using Services.Common;
using UnityEngine;

namespace Services.Logger
{
    public interface ILoggerService : IService
    {
        void Log(string message, LogType logType = LogType.Log);
    }
}