////////////////////////////////////////////////////////////////////////////////
//
// @author Benoît Freslon @benoitfreslon
// https://github.com/BenoitFreslon/Vibration
// https://benoitfreslon.com
// Скрипт с изменениями от оригинала
//
////////////////////////////////////////////////////////////////////////////////

// ReSharper disable RedundantUsingDirective

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Services.Vibration
{
    public static class Vibration
    {
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern bool _HasVibrator();

    [DllImport("__Internal")]
    private static extern void _Vibrate();

    [DllImport("__Internal")]
    private static extern void _VibratePop();

    [DllImport("__Internal")]
    private static extern void _VibratePeek();

    [DllImport("__Internal")]
    private static extern void _VibrateNope();
#endif

#if UNITY_ANDROID
        /*private static AndroidJavaClass _unityPlayer;
        private static AndroidJavaObject _currentActivity;
        private static AndroidJavaObject _vibrator;
        private static AndroidJavaObject _context;
        private static AndroidJavaClass _vibrationEffect;

        public static int AndroidVersion
        {
            get
            {
                var iVersionNumber = 0;
                if (Application.platform != RuntimePlatform.Android) return iVersionNumber;
            
                var androidVersion = SystemInfo.operatingSystem;
                var sdkPos = androidVersion.IndexOf("API-", StringComparison.Ordinal);
                iVersionNumber = int.Parse(androidVersion.Substring(sdkPos + 4, 2));
                return iVersionNumber;
            }
        }*/

#endif

        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;

#if UNITY_ANDROID
            /*if (Application.isMobilePlatform)
            {
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = _currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                _context = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");

                if (AndroidVersion >= 26)
                {
                    _vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                }
            }*/
#endif

            _initialized = true;
        }

        ///<summary>
        /// Tiny pop vibration
        ///</summary>
        public static void VibratePop()
        {
#if UNITY_IOS
            _VibratePop();
#elif UNITY_ANDROID
            Vibrate(50);
#endif
        }

        ///<summary>
        /// Small peek vibration
        ///</summary>
        public static void VibratePeek()
        {
#if UNITY_IOS
            _VibratePeek();
#elif UNITY_ANDROID
            Vibrate(100);
#endif
        }

        ///<summary>
        /// 3 small vibrations
        ///</summary>
        public static void VibrateNope()
        {
#if UNITY_IOS
            _VibrateNope();
#elif UNITY_ANDROID
            long[] pattern = { 0, 50, 50, 50 };
            Vibrate(pattern, -1);
#endif
        }

        ///<summary>
        /// Only on Android
        /// https://developer.android.com/reference/android/os/Vibrator.html#vibrate(long)
        ///</summary>
        public static void Vibrate(long milliseconds = 400)
        {
#if UNITY_ANDROID
            /*if (AndroidVersion >= 26)
            {
                var createOneShot = _vibrationEffect
                    .CallStatic<AndroidJavaObject>("createOneShot", milliseconds, -1);
                _vibrator.Call("vibrate", createOneShot);
            }
            else
            {
                _vibrator.Call("vibrate", milliseconds);
            }*/
#elif UNITY_IOS
            _Vibrate();
#endif
        }

        ///<summary>
        /// Only on Android
        /// https://proandroiddev.com/using-vibrate-in-android-b0e3ef5d5e07
        ///</summary>
        public static void Vibrate(long[] pattern, int repeat)
        {
#if UNITY_ANDROID
            /*if (AndroidVersion >= 26)
            {
                var createWaveform = _vibrationEffect
                    .CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
                _vibrator.Call("vibrate", createWaveform);
            }
            else
            {
                _vibrator.Call("vibrate", pattern, repeat);
            }*/
#elif UNITY_IOS
            _Vibrate();
#endif
        }

        ///<summary>
        ///Only on Android
        ///</summary>
        internal static void Cancel()
        {
#if UNITY_ANDROID
            //_vibrator.Call("cancel");
#endif
        }

        public static bool HasVibrator()
        {
#if UNITY_ANDROID
            /*var contextClass = new AndroidJavaClass("android.content.Context");
            var contextVibratorService = contextClass.GetStatic<string>("VIBRATOR_SERVICE");
            var systemService = _context
                .Call<AndroidJavaObject>("getSystemService", contextVibratorService);
            return systemService.Call<bool>("hasVibrator");
            */
            return false;

#elif UNITY_IOS
            return _HasVibrator();
#else
        return false;
#endif
        }

        public static void UnityVibrate()
        {
#if UNITY_ANDROID || UNITY_IOS
            //Handheld.Vibrate();
#endif
        }
    }
}