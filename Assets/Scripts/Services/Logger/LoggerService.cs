using System;
using Services.Common;
using UnityEngine;

namespace Services.Logger
{
    public class LoggerService : Service, ILoggerService
    {
        public void Log(string message, LogType logType = LogType.Log)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;

                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;

                case LogType.Warning:
#if DEV_ENV
                    Debug.LogWarning(message);
#endif
                    break;

                case LogType.Log:
#if DEV_ENV
                    Debug.Log(message);
#endif
                    break;

                case LogType.Exception:
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, message);
            }
        }
    }
}