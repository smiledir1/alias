using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;

namespace Services.Localization
{
    public interface ILocalizationService : IService
    {
        string GetText(string key);
        UniTask ChangeLanguage(SystemLanguage language);
    }
}