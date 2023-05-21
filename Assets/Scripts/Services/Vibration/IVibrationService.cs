using Services.Common;

namespace Services.Vibration
{
    public interface IVibrationService : IService
    {
        /// <summary>
        /// Turn Off or On 
        /// </summary>      
        void ChangeState(bool isOn);

        void CancelVibrate();

        /// <summary>
        /// Unity Vibration not work on iOS
        /// </summary>
        void SimpleVibrate();

        /// <summary>
        /// Classic Vibration 
        /// </summary>
        // 
        void DefaultVibrate();

        /// <summary>
        /// Pop vibration: weak boom
        /// </summary>
        // 
        void PopVibrate();

        /// <summary>
        /// Peek vibration: strong boom
        /// </summary>
        // 
        void PeekVibrate();

        /// <summary>
        /// Nope vibration: series of three weak booms
        /// </summary>
        void NopeVibrate();
    }
}