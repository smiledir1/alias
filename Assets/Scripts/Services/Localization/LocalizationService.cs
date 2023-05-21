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

        private const string _languageKey = "currentLanguage";

        #endregion

        private readonly IAssetsService _assetsService;

        private readonly Dictionary<string, string> _localizationSet = new();
        private LocalizationData _localizationData;
        private SystemLanguage _language;

        public LocalizationService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            // load all
            _localizationData = await _assetsService.LoadAsset<LocalizationData>();
            _language = (SystemLanguage) PlayerPrefs.GetInt(_languageKey, (int) SystemLanguage.English);
            await FillLocalizationSet(_language);
        }

        private async UniTask FillLocalizationSet(SystemLanguage language)
        {
            _localizationSet.Clear();
            for (var i = 0; i < _localizationData.UsesLanguages.Count; i++)
            {
                if (language != _localizationData.UsesLanguages[i]) continue;
                var languageDataReference = _localizationData.Languages[i];
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

        public async UniTask ChangeLanguage(SystemLanguage language)
        {
            _language = language;
            await FillLocalizationSet(language);

            PlayerPrefs.SetInt(_languageKey, (int) language);
            PlayerPrefs.Save();
        }
    }
}