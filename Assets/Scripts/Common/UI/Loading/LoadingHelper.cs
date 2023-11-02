using TMPro;
using UnityEngine;

namespace Common.UI.Loading
{
    public class LoadingHelper : MonoBehaviour
    {
        private const string LanguageKey = "currentLanguage";

        [SerializeField]
        private TextMeshProUGUI text;

        public void Initialize()
        {
            text.text = GetText();
        }

        private string GetText()
        {
            var language = (SystemLanguage) PlayerPrefs.GetInt(LanguageKey, (int) Application.systemLanguage);
            return language switch
            {
                SystemLanguage.English => "Loading...",
                SystemLanguage.Russian => "Загрузка...",
                SystemLanguage.Belarusian => "Загрузка...",
                // SystemLanguage.Chinese => "加载中",
                // SystemLanguage.ChineseTraditional => "載入中",
                // SystemLanguage.ChineseSimplified => "加载中",
                SystemLanguage.Finnish => "Ladataan...",
                SystemLanguage.French => "Chargement...",
                SystemLanguage.German => "Wird geladen...",
                //SystemLanguage.Hindi => "लोड हो रहा है",
                SystemLanguage.Italian => "Caricamento...",
                //SystemLanguage.Japanese => "読み込み中",
                //SystemLanguage.Korean => "로드 중",
                SystemLanguage.Portuguese => "Carregando...",
                SystemLanguage.Spanish => "Cargando...",
                SystemLanguage.Swedish => "Läser in...",
                SystemLanguage.Turkish => "Yükleniyor...",
                _ => "Loading..."
            };
        }
    }
}