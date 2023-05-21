namespace Services.Localization
{
    [System.Serializable]
    public class LocalizationEntry
    {
        public string Key;
        public string Text;

        public LocalizationEntry(string key, string text)
        {
            Key = key;
            Text = text;
        }
    }
}