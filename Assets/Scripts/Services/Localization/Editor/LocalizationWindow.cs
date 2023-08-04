#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Services.Localization.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        private const string PathToConfigs = "Assets/Configs";

        private static LocalizationWindow _window;

        private static string[] _keys;
        private static string[,] _translations;

        private static SystemLanguage _currentAddLanguage;
        private static string _currentAddLocalizeLanguage;
        private static string _searchText;
        private static Vector2 _scrollPos;
        private static string _spreadsheetUrl;

        private static GUIStyle _centeredLabel;
        private static GUIStyle _searchStyle;
        private static LocalizationData _localizationData;

        private bool _isGoogleTranslateOpen;
        private int _googleFromLanguage;
        private int _googleToLanguage;
        private string _googleFromText;
        private string _googleToText;
        private int _googleTextHeight = 20;
        private static readonly string[] _googleTranslateLanguages = {"ru", "en", "de", "fi", "fr", "it", "ja", "ko",
            "zh", "es", "sv", "tr",};

        [MenuItem("Tools/Localization Window")]
        private static void InitializeWindow()
        {
            _window = GetWindow<LocalizationWindow>();
            _window.Show();

            _centeredLabel = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter
            };

            _searchStyle = new GUIStyle("SearchTextField");

            var guids = AssetDatabase.FindAssets(
                $"t:{nameof(LocalizationData)}",
                new[] {PathToConfigs});
            if (guids.Length == 0)
            {
                CreateLocalizationData();
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);

            FillLanguages();
        }

        private void OnGUI()
        {
            if (_window == null) InitializeWindow();
            var serializeWindowObject = new SerializedObject(this);

            DrawTopMenu();

            DrawScrollView();

            DrawCheckLabel();

            DrawButtonMenu();

            DrawHelpersElements();

            serializeWindowObject.ApplyModifiedProperties();
        }

        private void DrawTopMenu()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Import CSV"))
            {
                ImportCSV();
            }

            if (GUILayout.Button("Export CSV"))
            {
                ExportCSV();
            }

            EditorGUILayout.EndHorizontal();

            //TODO: 
            // EditorGUILayout.BeginHorizontal();
            //
            // _currentAddLanguage = (SystemLanguage) EditorGUILayout.EnumPopup("Language", _currentAddLanguage);
            //
            // _currentAddLocalizeLanguage = EditorGUILayout.TextField(_currentAddLocalizeLanguage);
            //
            // if (GUILayout.Button("Add"))
            // {
            //     AddLanguage();
            // }
            //
            // EditorGUILayout.EndHorizontal();

            _searchText = EditorGUILayout.TextField(_searchText, _searchStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", _centeredLabel);
            foreach (var localizationDataItem in _localizationData.Languages)
            {
                var currentLanguage = localizationDataItem.SystemLanguage.ToString();
                EditorGUILayout.LabelField(
                    $"{currentLanguage} ({localizationDataItem.LanguageLocalizeName})",
                    _centeredLabel);
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void CreateLocalizationData()
        {
            //TODO:
        }

        private static void FillLanguages()
        {
            if (_localizationData == null || _localizationData.Languages.Count == 0) return;
            var firstEntries = _localizationData.Languages[0].LanguageWords.editorAsset.Entries;
            var entryCount = firstEntries.Count;
            var localizationsCount = _localizationData.Languages.Count;
            _keys = new string[entryCount];
            _translations = new string[entryCount, localizationsCount];

            for (var i = 0; i < firstEntries.Count; i++)
            {
                var entry = firstEntries[i];
                _keys[i] = entry.Key;
            }

            for (var i = 0; i < localizationsCount; i++)
            {
                var entries = _localizationData.Languages[i].LanguageWords.editorAsset.Entries;
                for (var j = 0; j < entries.Count; j++)
                {
                    if (j >= entryCount) break;

                    var entry = entries[j];
                    _translations[j, i] = entry.Text;
                    if (string.Compare(_keys[j], entry.Key, StringComparison.Ordinal) != 0)
                    {
                        Debug.LogWarning(
                            $"{_localizationData.Languages[i].SystemLanguage}" +
                            $" {_keys[j]} {entry.Key} {i} {j} different keys");
                    }
                }
            }
        }

        private void ImportCSV()
        {
            var path = $"{Application.dataPath}/loc.csv";
            var text = File.ReadAllText(path, Encoding.UTF8);


            const char RowSeparator = ';';
            const char ColumnSeparator = '\n';
            ImportFromText(text, RowSeparator, ColumnSeparator);

            Debug.Log("Import complete");
        }

        private void ImportFromText(string text, char rowSeparator, char columnSeparator)
        {
            //TODO: проверить порядок

            var lines = text.Split(columnSeparator);
            var header = lines[0].Split(rowSeparator);

            _keys = new string[lines.Length - 1];
            _translations = new string[lines.Length - 1, header.Length - 1];
            for (var i = 1; i < lines.Length; ++i)
            {
                var items = lines[i].Split(rowSeparator);
                var pos = i - 1;
                _keys[pos] = items[0];
                for (var j = 1; j < items.Length; j++)
                {
                    _translations[pos, j - 1] = items[j];
                }
            }
        }

        private void ExportCSV()
        {
            const char RowSeparator = ';';
            const char ColumnSeparator = '\n';

            var path = $"{Application.dataPath}/locExport.csv";
            var exportTextBuilder = new StringBuilder();

            exportTextBuilder.Append(RowSeparator);
            var languagesLastCount = _localizationData.Languages.Count - 1;

            for (var i = 0; i < _localizationData.Languages.Count; i++)
            {
                var localization = _localizationData.Languages[i];
                var currentLanguage = localization.SystemLanguage.ToString();
                exportTextBuilder.Append(currentLanguage);
                if (i != languagesLastCount) exportTextBuilder.Append(RowSeparator);
            }

            exportTextBuilder.Append(ColumnSeparator);

            var languagesCount = _translations.GetLength(1);
            var keysLastCount = _keys.Length - 1;
            for (var i = 0; i < _keys.Length; i++)
            {
                exportTextBuilder.Append(_keys[i]);
                exportTextBuilder.Append(RowSeparator);
                var languagesCountLast = languagesCount - 1;
                for (var j = 0; j < languagesCount; j++)
                {
                    exportTextBuilder.Append(_translations[i, j]);
                    if (j != languagesCountLast) exportTextBuilder.Append(RowSeparator);
                }

                if (i != keysLastCount) exportTextBuilder.Append(ColumnSeparator);
            }

            File.WriteAllText(path, exportTextBuilder.ToString(), Encoding.UTF8);

            Debug.Log("Export complete");
        }

        private void AddLanguage()
        {
            //TODO:
        }

        private void DrawScrollView()
        {
            if (_localizationData.Languages.Count == 0) return;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var languagesCount = _localizationData.Languages.Count;
            for (var i = 0; i < _keys.Length; i++)
            {
                if (!HasCheckSearchStr(i, languagesCount)) continue;

                EditorGUILayout.BeginHorizontal();

                _keys[i] = EditorGUILayout.TextField(_keys[i]);
                for (var j = 0; j < languagesCount; j++)
                {
                    _translations[i, j] = EditorGUILayout.TextField(_translations[i, j]);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawButtonMenu()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key"))
            {
                AddKey();
            }

            if (GUILayout.Button("Remove Last Key"))
            {
                RemoveLastKey();
            }

            if (GUILayout.Button("Up"))
            {
                Up();
            }

            if (GUILayout.Button("Down"))
            {
                Down();
            }

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void AddKey()
        {
            var newKeys = new string[_keys.Length + 1];
            for (var i = 0; i < _keys.Length; i++)
            {
                newKeys[i] = _keys[i];
            }

            _keys = newKeys;

            var languagesCount = _localizationData.Languages.Count;
            var translationsLengthI = _translations.GetLength(1);
            var translationsLengthJ = _translations.GetLength(0);
            var newTranslations = new string[translationsLengthJ + 1, translationsLengthI];
            for (var i = 0; i < languagesCount; i++)
            {
                for (var j = 0; j < translationsLengthJ; j++)
                {
                    newTranslations[j, i] = _translations[j, i];
                }

                newTranslations[translationsLengthJ, i] = string.Empty;
            }

            _translations = newTranslations;
        }

        private void RemoveLastKey()
        {
            var newKeys = new string[_keys.Length - 1];
            for (var i = 0; i < newKeys.Length; i++)
            {
                newKeys[i] = _keys[i];
            }

            _keys = newKeys;

            var languagesCount = _localizationData.Languages.Count;
            var translationsLengthI = _translations.GetLength(1);
            var translationsLengthJ = _translations.GetLength(0);
            var newTranslationsJ = translationsLengthJ - 1;
            var newTranslations = new string[newTranslationsJ, translationsLengthI];
            for (var i = 0; i < languagesCount; i++)
            {
                for (var j = 0; j < newTranslationsJ; j++)
                {
                    newTranslations[j, i] = _translations[j, i];
                }
            }

            _translations = newTranslations;
        }

        private void Up()
        {
            _scrollPos += new Vector2(0f, -20f);
        }

        private void Down()
        {
            _scrollPos += new Vector2(0f, 20f);
        }

        private void Save()
        {
            if (_localizationData == null || _localizationData.Languages.Count == 0) return;
            var keysCount = _keys.Length;
            var languagesCount = _localizationData.Languages.Count;
            for (var i = 0; i < languagesCount; i++)
            {
                var language = _localizationData.Languages[i];
                var entries = language.LanguageWords.editorAsset.Entries;
                entries.Clear();
                for (var j = 0; j < keysCount; j++)
                {
                    var key = _keys[j];
                    var text = _translations[j, i];
                    var newEntry = new LocalizationEntry(key, text);
                    entries.Add(newEntry);
                }

                EditorUtility.SetDirty(language.LanguageWords.editorAsset);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Save complete");
        }

        private void DrawCheckLabel()
        {
            //TODO:
        }

        private bool HasCheckSearchStr(int i, int languagesCount)
        {
            if (string.IsNullOrWhiteSpace(_searchText)) return true;

            if (_keys[i].Contains(_searchText, StringComparison.Ordinal))
            {
                return true;
            }

            for (var j = 0; j < languagesCount; j++)
            {
                if (_translations[i, j].Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void DownloadCsv()
        {
            DownloadAndParseTable().Forget();
        }

        private async UniTask DownloadAndParseTable()
        {
            if (!_spreadsheetUrl.EndsWith("tsv"))
            {
                var lastSlash = _spreadsheetUrl.LastIndexOf('/') + 1;
                _spreadsheetUrl = _spreadsheetUrl.Substring(0, lastSlash);
                _spreadsheetUrl += "export?format=tsv";
            }

            var parseData = await DownloadTable(_spreadsheetUrl);

            const char RowSeparator = '\t';
            const char ColumnSeparator = '\n';
            ImportFromText(parseData, RowSeparator, ColumnSeparator);
        }


        private static async UniTask<string> DownloadTable(string actualUrl)
        {
            using var request = UnityWebRequest.Get(actualUrl);
            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError(request.error);
                return string.Empty;
            }

            Debug.Log("Successful download");
            return request.downloadHandler.text;
        }

        private void DrawGoogleSpreadSheetDownload()
        {
            EditorGUILayout.BeginHorizontal();
            var labelText =
                "CSV URL example: " +
                "https://docs.google.com/spreadsheets/d/[SPREADSHEETHASH]/export?format=tsv";
            EditorGUILayout.LabelField(labelText, GUI.tooltip);

            _spreadsheetUrl = EditorGUILayout.TextField(_spreadsheetUrl);

            if (GUILayout.Button("Download CSV"))
            {
                DownloadCsv();
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawHelpersElements()
        {
            var buttonOpenText = _isGoogleTranslateOpen ? "Close Helpers" : "Open Helpers";
            if (GUILayout.Button(buttonOpenText)) _isGoogleTranslateOpen = !_isGoogleTranslateOpen;
            if (!_isGoogleTranslateOpen) return;

            DrawGoogleSpreadSheetDownload();
            DrawGoogleLocalization();
            DrawKeyConverter();
        }

        private void DrawGoogleLocalization()
        {
            _googleTextHeight = EditorGUILayout.IntSlider(
                "Text Height", _googleTextHeight, 20, 500);
            
            EditorGUILayout.BeginHorizontal();

            _googleFromLanguage = EditorGUILayout.Popup(_googleFromLanguage, _googleTranslateLanguages);
            _googleToLanguage = EditorGUILayout.Popup(_googleToLanguage, _googleTranslateLanguages);
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();

            var width = _window.position.width / 2;
            _googleFromText = EditorGUILayout.TextArea(_googleFromText, 
                GUILayout.Height(_googleTextHeight), GUILayout.Width(width));
            _googleToText = EditorGUILayout.TextArea(_googleToText, 
                GUILayout.Height(_googleTextHeight), GUILayout.Width(width));
            
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Translate")) TranslateFromGoogle().Forget();
        }

        private async UniTask TranslateFromGoogle()
        {
            var fromLanguage = _googleTranslateLanguages[_googleFromLanguage];
            var toLanguage = _googleTranslateLanguages[_googleToLanguage];
            var translateText = WebUtility.UrlEncode(_googleFromText);
            var url =
                $"https://translate.google.com/translate_a/single" +
                $"?client=gtx&dt=t&sl={fromLanguage}&tl={toLanguage}&q={translateText}";

            var webRequest = UnityWebRequest.Get(url);

            try
            {
                var response = await webRequest.SendWebRequest();
                var responseText = response.downloadHandler.text;
                var jsonArray = JArray.Parse(responseText);
                _googleToText = jsonArray[0][0][0].ToString();
                Debug.Log("Translate done");
            }
            catch
            {
                Debug.LogError("(enMessage) The process is not completed! Most likely, you made too " +
                               "many requests. In this case, the Google Translate API blocks access to the " +
                               "translation for a while.  Please try again later. Do not translate the text " +
                               "too often, so that Google does not consider your actions as spam");
            }
        }

        private string _fromKeyConverter;
        private string _toKeyConverter;
        
        private void DrawKeyConverter()
        {
            EditorGUILayout.BeginHorizontal();
            _fromKeyConverter = EditorGUILayout.TextField(_fromKeyConverter);
            _toKeyConverter = EditorGUILayout.TextField(_toKeyConverter);
            if (GUILayout.Button("Convert to key"))
            {
                _toKeyConverter = _fromKeyConverter.ToLower().Replace(" ", "_");
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif