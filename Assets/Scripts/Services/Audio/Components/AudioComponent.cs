using Services.Helper;
using UnityEngine;

namespace Services.Audio.Components
{
    public abstract class AudioComponent : MonoBehaviour
    {
        [Service]
        protected static IAudioService AudioService;

        [SerializeField]
        protected string _soundKey;

        [SerializeField]
        protected bool _multiSound;

        protected void PlaySound()
        {
            AudioService.PlaySound(_soundKey, _multiSound);
        }

#if UNITY_EDITOR
        public string SoundKey
        {
            get => _soundKey;
            set => _soundKey = value;
        }
#endif
    }
}