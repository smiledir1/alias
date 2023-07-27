using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;

namespace Services.Localization
{
    public interface ILocalizationService : IService
    {
        SystemLanguage CurrentLanguage { get; }
        string CurrentLanguageLocalizeName { get; }
        string GetText(string key);
        UniTask ChangeLanguage(SystemLanguage language);
        UniTask<SystemLanguage> ChangeLanguage();
        string GetFormattedText(string key, params object[] parameters);
    }
}