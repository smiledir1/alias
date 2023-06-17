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
        private TextMeshProUGUI _label;
        
        [SerializeField]
        private Button _button;

        public void Initialize(string labelText, UnityAction onClick)
        {
            _label.text = labelText;
            _button.SetClickListener(onClick);
        }
    }
}