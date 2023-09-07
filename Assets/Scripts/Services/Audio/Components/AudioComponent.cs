using Services.Helper;
using UnityEngine;

namespace Services.Audio.Components
{
    public abstract class AudioComponent : MonoBehaviour
    {
        [Service]
        protected static IAudioService AudioService;

        [SerializeField]
        protected string soundKey;

        [SerializeField]
        protected bool multiSound;

        protected void PlaySound()
        {
            AudioService.PlaySound(soundKey, multiSound);
        }

#if UNITY_EDITOR
        public string SoundKey
        {
            get => soundKey;
            set => soundKey = value;
        }
#endif
    }
}