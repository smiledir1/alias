using Services.Helper;
using TMPro;
using UnityEngine;

namespace Services.Localization.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPLocalization : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        private bool recalculateOnEnable = true;

        [SerializeField]
        private string key;

        [Service]
        private static ILocalizationService _localizationService;

        private void Awake()
        {
            if (recalculateOnEnable) return;
            Recalculate();
        }

        private void OnEnable()
        {
            if (!recalculateOnEnable) return;
            Recalculate();
        }

        public void Recalculate()
        {
            if (text == null) return;
            text.text = _localizationService?.GetText(key);
        }

#if UNITY_EDITOR
        public TextMeshProUGUI Text => text;

        public string Key
        {
            get => key;
            set => key = value;
        }

        private void OnValidate()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}