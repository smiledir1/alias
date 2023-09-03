using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;
using UnityEngine;

namespace Services.Localization
{
    public class LocalizationService : Service, ILocalizationService
    {
        #region Const

        private const string LanguageKey = "currentLanguage";

        #endregion

        private readonly IAssetsService _assetsService;

        private readonly Dictionary<string, string> _localizationSet = new();
        private LocalizationData _localizationData;
        private SystemLanguage _language;

        public SystemLanguage CurrentLanguage => _language;

        public string CurrentLanguageLocalizeName =>
            _localizationData.Languages.Find(
                    l => l.SystemLanguage == _language)
                .LanguageLocalizeName;


        public LocalizationService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);
            
            _localizationData = await _assetsService.LoadAsset<LocalizationData>();

            var systemLanguage = SystemLanguage.English;
            if (!PlayerPrefs.HasKey(LanguageKey))
            {
                var applicationSystemLanguage = Application.systemLanguage;
                foreach (var language in _localizationData.Languages)
                {
                    if (language.SystemLanguage != applicationSystemLanguage) continue;
                    systemLanguage = applicationSystemLanguage;
                    break;
                }
                PlayerPrefs.SetInt(LanguageKey, (int) systemLanguage);
                PlayerPrefs.Save();
            }

            _language = (SystemLanguage) PlayerPrefs.GetInt(LanguageKey, (int) systemLanguage);
            await FillLocalizationSet(_language);
        }

        private async UniTask FillLocalizationSet(SystemLanguage language)
        {
            _localizationSet.Clear();
            for (var i = 0; i < _localizationData.Languages.Count; i++)
            {
                var languageDataItem = _localizationData.Languages[i];
                if (languageDataItem.SystemLanguage != language) continue;

                var languageDataReference = languageDataItem.LanguageWords;
                var languageData = languageDataReference.Asset == null
                    ? await languageDataReference.LoadAssetAsync()
                    : languageDataReference.Asset as LanguageEntry;
                if (languageData == null) return;
                foreach (var entry in languageData.Entries)
                {
                    _localizationSet.Add(entry.Key, entry.Text);
                }
            }
        }

        public string GetText(string key)
        {
            return _localizationSet.TryGetValue(key, out var text) ? text : key;
        }
        
        public string GetFormattedText(string key, params object[] parameters)
        {
            var localizeText = _localizationSet.TryGetValue(key, out var text) ? text : key;
            return string.Format(localizeText, parameters);
        }

        public async UniTask ChangeLanguage(SystemLanguage language)
        {
            _language = language;
            await FillLocalizationSet(language);

            PlayerPrefs.SetInt(LanguageKey, (int) language);
            PlayerPrefs.Save();
        }

        public async UniTask<SystemLanguage> ChangeLanguage()
        {
            var languages = _localizationData.Languages;
            var currentLanguageIndex = languages.FindIndex(
                l => l.SystemLanguage == _language);
            currentLanguageIndex++;
            if (currentLanguageIndex == languages.Count) currentLanguageIndex = 0;
            var language = languages[currentLanguageIndex].SystemLanguage;
            _language = language;
            await FillLocalizationSet(language);

            PlayerPrefs.SetInt(LanguageKey, (int) language);
            PlayerPrefs.Save();
            return language;
        }
    }
}