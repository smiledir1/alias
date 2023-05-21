using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;

namespace Services.Vibration
{
    public class VibrationService : Service, IVibrationService
    {
        private const string _savePrefKey = "vibration";
        public bool IsOn { get; private set; }

        protected override UniTask OnInitialize()
        {
            Vibration.Init();
            var state = PlayerPrefs.GetInt(_savePrefKey, 1);
            IsOn = state == 1;
            return base.OnInitialize();
        }

        /// <summary>
        /// Turn Off or On 
        /// </summary>      
        public void ChangeState(bool isOn)
        {
            IsOn = isOn;
            var state = isOn ? 1 : 0;
            PlayerPrefs.SetInt(_savePrefKey, state);
            PlayerPrefs.Save();
        }

        public void CancelVibrate()
        {
            if (!IsOn) return;
            Vibration.Cancel();
        }

        /// <summary>
        /// Unity Vibration not work on iOS
        /// </summary>
        public void SimpleVibrate()
        {
            if (!IsOn) return;
            Vibration.UnityVibrate();
        }

        /// <summary>
        /// Classic Vibration 
        /// </summary>
        // 
        public void DefaultVibrate()
        {
            if (!IsOn) return;
            Vibration.Vibrate();
        }

        /// <summary>
        /// Pop vibration: weak boom
        /// </summary>
        // 
        public void PopVibrate()
        {
            if (!IsOn) return;
            Vibration.VibratePop();
        }

        /// <summary>
        /// Peek vibration: strong boom
        /// </summary>
        // 
        public void PeekVibrate()
        {
            if (!IsOn) return;
            Vibration.VibratePeek();
        }

        /// <summary>
        /// Nope vibration: series of three weak booms
        /// </summary>
        public void NopeVibrate()
        {
            if (!IsOn) return;
            Vibration.VibrateNope();
        }
    }
}