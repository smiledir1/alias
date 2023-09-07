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
        private TextMeshProUGUI label;

        [SerializeField]
        private TMP_InputField input;

        [SerializeField]
        private Button button;

        public void Initialize(string labelText, string inputText, UnityAction<string> onClick)
        {
            label.text = labelText;
            input.text = inputText;
            button.SetClickListener(() => { onClick?.Invoke(input.text); });
        }
    }
}