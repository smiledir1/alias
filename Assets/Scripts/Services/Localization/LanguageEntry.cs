using System.Collections.Generic;
using UnityEngine;

namespace Services.Localization
{
    [CreateAssetMenu(menuName = "Services/Localization/LocalizationLanguage", fileName = "Localization")]
    public class LanguageEntry : ScriptableObject
    {
        [SerializeField]
        private SystemLanguage language;
        
        [SerializeField]
        private List<LocalizationEntry> entries = new();
        
        public SystemLanguage Language => language;
        public List<LocalizationEntry> Entries => entries;
    }
}