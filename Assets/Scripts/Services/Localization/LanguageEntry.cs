using System.Collections.Generic;
using UnityEngine;

namespace Services.Localization
{
    [CreateAssetMenu(menuName = "Services/Localization/LocalizationLanguage", fileName = "Localization")]
    public class LanguageEntry : ScriptableObject
    {
        public SystemLanguage Language;
        public List<LocalizationEntry> Entries = new();
    }
}