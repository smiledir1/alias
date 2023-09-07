using TMPro;
using UnityEngine;

namespace Common.Cheats
{
    public class CheatsLabel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI label;

        public void Initialize(string labelText)
        {
            label.text = labelText;
        }
    }
}