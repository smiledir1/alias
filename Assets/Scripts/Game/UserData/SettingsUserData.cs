using System;
using Services.UserData;

namespace Game.UserData
{
    [Serializable]
    public class SettingsUserData : UserDataObject
    {
        public override string DataName => nameof(SettingsUserData);

        public bool SoundEnabled = true;
        public bool MusicEnabled = true;
        public bool VibrationEnabled = true;
    }
}