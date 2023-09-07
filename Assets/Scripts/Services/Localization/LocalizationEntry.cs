using UnityEngine;

namespace Services.Localization
{
    [System.Serializable]
    public class LocalizationEntry
    {
        [SerializeField]
        public string key;

        [SerializeField]
        public string text;

        public string Key => key;
        public string Text => text;

        public LocalizationEntry(string key, string text)
        {
            this.key = key;
            this.text = text;
        }
    }
}