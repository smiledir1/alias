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
        private SystemLanguage systemLanguage;

        [SerializeField]
        private string languageLocalizeName;

        [SerializeField]
        private AssetReferenceT<LanguageEntry> languageWords;

        public SystemLanguage SystemLanguage => systemLanguage;
        public string LanguageLocalizeName => languageLocalizeName;
        public AssetReferenceT<LanguageEntry> LanguageWords => languageWords;
    }
}