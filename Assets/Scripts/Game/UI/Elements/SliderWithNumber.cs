using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Elements
{
    public class SliderWithNumber : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _number;

        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private int _roundDigits;

        private void Awake()
        {
            OnValueChanged(_slider.value);
            _slider.onValueChanged.RemoveAllListeners();
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            _number.text = value.ToString($"F{_roundDigits}");
        }
    }
}