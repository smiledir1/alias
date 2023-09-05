using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Localization
{
    [CreateAssetMenu(menuName = "Services/Localization/LocalizationsConfig", fileName = "LocalizationsConfig")]
    public class LocalizationData : ScriptableObject
    {
#if UNITY_EDITOR
        [Header("https://docs.google.com/spreadsheets/d/[SPREADSHEETHASH]/export?format=csv")]
        [SerializeField]
        private string _spreadsheetUrl;

        public string SpreadsheetUrl => _spreadsheetUrl;
#endif
        public List<LocalizationDataItem> Languages = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var language in Languages)
            {
                if (language == null) continue;
                language.SystemLanguage = language.LanguageWords.editorAsset.Language;
            }
        }
#endif
    }

    [Serializable]
    public class LocalizationDataItem
    {
        public SystemLanguage SystemLanguage;
        public string LanguageLocalizeName;
        public AssetReferenceT<LanguageEntry> LanguageWords;
    }
}