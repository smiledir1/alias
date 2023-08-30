using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Services.Audio.Components
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : AudioComponent, IPointerClickHandler
    {
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            ButtonPress();
        }

        private void ButtonPress()
        {
            PlaySound();
        }
    }
}