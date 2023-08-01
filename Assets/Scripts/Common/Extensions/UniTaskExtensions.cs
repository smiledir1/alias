using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.Extensions
{
    public static class UniTaskExtensions
    {
        public static void SafeForget(this UniTask task)
        {
            if (task.Status != UniTaskStatus.Pending) return;

            var message = GetSourceCall();
            task.Forget(Handler);

            void Handler(Exception exception)
            {
                switch (exception)
                {
                    case OperationCanceledException _:
                        // operation cancelled is ok
                        break;
                    default:
                        Debug.LogError($"[UniTask] Task has completed with exception, check inner exception for details.\nSourceCall: {message}\n{exception.Message}\n{exception.StackTrace}");
                        break;
                }    
            }
        }

        public static void SafeForget<T>(this UniTask<T> task)
        {
            if (task.Status != UniTaskStatus.Pending) return;
            
            var message = GetSourceCall();
            task.Forget(Handler);

            void Handler(Exception exception)
            {

                switch (exception)
                {
                    case OperationCanceledException _:
                        // operation cancelled is ok
                        break;
                    default:
                        Debug.LogError($"[UniTask] Task has completed with, check inner exception for details.\nSourceCall: {message}\n{exception.Message}\n{exception.StackTrace}");
                        break;
                }    
            }
        }
        
        private static string GetSourceCall()
        {
#if DEv_ENV
            var frame = new System.Diagnostics.StackTrace(0, true).GetFrame(2);
            var filename = frame.GetFileName();
            var assetName = filename?.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            var line = frame.GetFileLineNumber();
            var assetPath = $"{assetName}:{line}";
            var message = $" {frame.GetMethod()} (at <a href=\"{assetName}\" line=\"{line}\">{assetPath}</a>)";
#else
            string message = " Inspect stack trace for source call.";
#endif
            return message;
        }
    }
}
