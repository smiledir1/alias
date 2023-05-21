using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Localization
{
    [CreateAssetMenu(menuName = "Services/Localization/LocalizationsConfig", fileName = "LocalizationsConfig")]
    public class LocalizationData : ScriptableObject
    {
#if UNITY_EDITOR
        [Header("https://docs.google.com/spreadsheets/d/SPREADSHEETHASH/export?format=csv")]
        [SerializeField]
        private string _spreadsheetUrl;

        public string SpreadsheetUrl => _spreadsheetUrl;
#endif
        public List<SystemLanguage> UsesLanguages = new();
        public List<AssetReferenceT<LanguageEntry>> Languages = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            UsesLanguages.Clear();
            foreach (var language in Languages)
            {
                if(language == null) continue;
                UsesLanguages.Add(language.editorAsset.Language);
            }
        }
#endif
    }
}