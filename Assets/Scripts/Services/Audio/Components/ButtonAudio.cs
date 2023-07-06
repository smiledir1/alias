using Services.Helper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Services.Audio.Components
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : MonoBehaviour, IPointerClickHandler
    {
        [Service]
        private static IAudioService _audioService;
        
        [SerializeField]
        private string _soundKey;
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            ButtonPress();
        }

        private void ButtonPress()
        {
            //TODO: check key in config, write if not, help find keys
            _audioService.PlaySound(_soundKey);
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