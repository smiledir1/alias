using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Common.Extensions
{
    public static class EventExtensions
    {
        private static UniTaskCompletionSource _completionSource;
        
        /// <summary>
        /// Использовать осторожно, не одновременно
        /// </summary>
        /// <param name="unityEvent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static UniTask WaitResult(this UnityEvent unityEvent, CancellationToken cancellationToken = default)
        {
            _completionSource = new UniTaskCompletionSource();
            cancellationToken.Register(() => _completionSource?.TrySetCanceled());

            unityEvent.AddListener(WaitCall);

            return _completionSource.Task;
        }

        private static void WaitCall()
        {
            _completionSource.TrySetResult();
        }
    }
}