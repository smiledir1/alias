using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Common.CheckNetwork
{
    public static class NetworkChecker
    {
        private const string ReachURL = "https://clients3.google.com/generate_204";
        private const float ReachTimeout = 2f;
        private static bool _isOnline;

        public static bool IsOnline() => 
            _isOnline && 
            Application.internetReachability != NetworkReachability.NotReachable;

        public static async UniTask<bool> CheckConnection()
        {
            _isOnline = false;
            var startTime = Time.realtimeSinceStartup;
            using var unityWebRequest = UnityWebRequest.Get(ReachURL);
            var request = unityWebRequest.SendWebRequest();
            var time = 0f;
            while (!request.isDone && time < ReachTimeout)
            {
                time = Time.realtimeSinceStartup - startTime;
                await UniTask.Yield();
            }

            if (unityWebRequest.result != UnityWebRequest.Result.Success || time > ReachTimeout)
            {
                _isOnline = false;
            }
            else
            {
                _isOnline = true;
            }

            return _isOnline;
        }
    }
}