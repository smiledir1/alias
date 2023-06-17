using System;
using Services.Helper;
using TMPro;
using UnityEngine;

namespace Services.Localization.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPLocalization : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private bool _recalculateOnEnable;

        [SerializeField]
        private string _key;

        [Service]
        private static ILocalizationService _localizationService;

        private void Awake()
        {
            if (_recalculateOnEnable) return;
            Recalculate();
        }

        private void OnEnable()
        {
            if (!_recalculateOnEnable) return;
            Recalculate();
        }

        public void Recalculate()
        {
            if (_text == null) return;
            _localizationService?.GetText(_key);
        }

#if UNITY_EDITOR
        public TextMeshProUGUI Text => _text;

        public string Key
        {
            get => _key;
            set => _key = value;
        }

        private void OnValidate()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}