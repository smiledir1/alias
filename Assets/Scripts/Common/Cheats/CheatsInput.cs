using Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Cheats
{
    public class CheatsInput : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _label;

        [SerializeField]
        private TMP_InputField _input;

        [SerializeField]
        private Button _button;

        public void Initialize(string labelText, string inputText, UnityAction<string> onClick)
        {
            _label.text = labelText;
            _input.text = inputText;
            _button.SetClickListener(() => { onClick?.Invoke(_input.text); });
        }
    }
}