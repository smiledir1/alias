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

        [ContextMenu("TranslateToNextLanguage")]
        private void TranslateToNextLanguage()
        {
            var task = TranslateToNextLanguageAsync();
            Cysharp.Threading.Tasks.UniTaskExtensions.Forget(task);
            
            async Cysharp.Threading.Tasks.UniTask TranslateToNextLanguageAsync()
            {
                await _localizationService.ChangeLanguage();
                var localizations = FindObjectsOfType<TMPLocalization>(true);
                foreach (var localization in localizations)
                {
                    localization.Recalculate();
                }
            }
        }
#endif
    }
}