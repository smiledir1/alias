using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Elements
{
    public class SliderWithNumber : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI number;

        [SerializeField]
        private Slider slider;

        [SerializeField]
        private int roundDigits;

        private void Awake()
        {
            OnValueChanged(slider.value);
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            number.text = value.ToString($"F{roundDigits}");
        }
    }
}