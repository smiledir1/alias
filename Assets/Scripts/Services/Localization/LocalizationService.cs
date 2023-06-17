﻿using System.Collections.Generic;
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

        public LocalizationService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            // load all
            _localizationData = await _assetsService.LoadAsset<LocalizationData>();
            _language = (SystemLanguage) PlayerPrefs.GetInt(LanguageKey, (int) SystemLanguage.English);
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

        public async UniTask ChangeLanguage(SystemLanguage language)
        {
            _language = language;
            await FillLocalizationSet(language);

            PlayerPrefs.SetInt(LanguageKey, (int) language);
            PlayerPrefs.Save();
        }
    }
}