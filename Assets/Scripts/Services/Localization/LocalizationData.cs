using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Localization
{
    [CreateAssetMenu(menuName = "Services/Localization/LocalizationsConfig", fileName = "LocalizationsConfig")]
    public class LocalizationData : ScriptableObject
    {
        [SerializeField]
        private List<LocalizationDataItem> languages = new();

        public List<LocalizationDataItem> Languages => languages;
    }

    [Serializable]
    public class LocalizationDataItem
    {
        [SerializeField]
        private SystemLanguage _systemLanguage;

        [SerializeField]
        private string _languageLocalizeName;

        [SerializeField]
        private AssetReferenceT<LanguageEntry> _languageWords;

        public SystemLanguage SystemLanguage => _systemLanguage;
        public string LanguageLocalizeName => _languageLocalizeName;
        public AssetReferenceT<LanguageEntry> LanguageWords => _languageWords;
    }
}