using Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Cheats
{
    public class CheatsButton : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI label;

        [SerializeField]
        private Button button;

        public void Initialize(string labelText, UnityAction onClick)
        {
            label.text = labelText;
            button.SetClickListener(onClick);
        }
    }
}