using TMPro;
using UnityEngine;

namespace Common.Cheats
{
    public class CheatsLabel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _label;
        
        public void Initialize(string labelText)
        {
            _label.text = labelText;
        }
    }
}