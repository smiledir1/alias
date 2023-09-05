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
        private const string CurrentPagePref = "current_page_localization";
        private const string WordsOnPagePref = "words_on_page_localization";

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
        private static GUIStyle _textAreaStyle;
        private static LocalizationData _localizationData;

        private bool _isHelpersOpen;
        private int _googleFromLanguage;
        private int _googleToLanguage;
        private string _googleFromText;
        private string _googleToText;
        private int _googleTextHeight = 20;

        private static readonly string[] GoogleTranslateLanguages =
        {
            "ru", "en", "de", "fi", "fr", "it", "ja", "ko",
            "zh", "es", "sv", "tr"
        };

        private string _fromKeyConverter;
        private string _toKeyConverter;
        //private bool _showScrollView;

        private static readonly Dictionary<int, bool> ShowLanguages = new();
        private bool _showChooseLanguages;

        private static int _generatedWordsCount;
        private static int _wordsCount;
        private static int _elementsOnPage = 10;
        private static int _currentPage;
        private int _pagesCount;

        [MenuItem("Tools/Localization/Create Localization Constants")]
        private static void CreateLocalizationConfigConstants()
        {
            var pathToScriptsDirectory = Path.Combine(Application.dataPath, "Scripts");
            var pathToGameDirectory = Path.Combine(pathToScriptsDirectory, "Game");
            var pathToConstantsDirectory = Path.Combine(pathToGameDirectory, "Constants");
            var pathToFileConstants = Path.Combine(pathToConstantsDirectory, "LocalizationConstants.cs");

            var fileText = "public static class LocalizationConstants {";

            var guids = AssetDatabase.FindAssets(
                $"t:{nameof(LocalizationData)}",
                new[] {PathToConfigs});
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);

            foreach (var entry in _localizationData.Languages[0].LanguageWords.editorAsset.Entries)
            {
                fileText += $"public const string {entry.Key}_l = \"{entry.Key}\"; ";
            }

            fileText += "}";

            Directory.CreateDirectory(pathToConstantsDirectory);
            File.WriteAllText(pathToFileConstants, fileText);
            AssetDatabase.Refresh();
            Debug.Log("Constants Created");
        }

        [MenuItem("Tools/Localization/Localization Window")]
        private static void InitializeWindow()
        {
            _window = GetWindow<LocalizationWindow>();
            _window.Show();

            _centeredLabel = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter
            };

            _searchStyle = new GUIStyle("SearchTextField");

            _textAreaStyle = new GUIStyle("TextArea")
            {
                wordWrap = true
            };

            _spreadsheetUrl = "https://docs.google.com/spreadsheets/d/[SPREADSHEETHASH]/export?format=tsv";

            var guids = AssetDatabase.FindAssets(
                $"t:{nameof(LocalizationData)}",
                new[] {PathToConfigs});
            if (guids.Length == 0) CreateLocalizationData();

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);

            FillLanguages();
        }

        private void OnGUI()
        {
            var e = Event.current;

            if (e.type == EventType.MouseUp && e.button == 1) Debug.Log("Right mouse button lifted");

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
            for (var i = 0; i < _localizationData.Languages.Count; i++)
            {
                if (!ShowLanguages[i]) continue;
                var localizationDataItem = _localizationData.Languages[i];
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
            _wordsCount = entryCount;

            for (var i = 0; i < firstEntries.Count; i++)
            {
                var entry = firstEntries[i];
                _keys[i] = entry.Key;
            }

            for (var i = 0; i < localizationsCount; i++)
            {
                var entries = _localizationData.Languages[i].LanguageWords.editorAsset.Entries;
                ShowLanguages.TryAdd(i, EditorPrefs.GetBool($"show_in_window_language_{i}", true));
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

            _currentPage = EditorPrefs.GetInt(CurrentPagePref, 1);
            _elementsOnPage = EditorPrefs.GetInt(WordsOnPagePref, 10);
        }

        private void ImportCsv()
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

        private void ExportCsv()
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

            // if (!_showScrollView)
            // {
            //     if (GUILayout.Button("Show Localization Entries"))
            //     {
            //         _showScrollView = true;
            //     }
            //
            //     return;
            // }

            var startWord = _elementsOnPage * (_currentPage - 1);
            var endWord = _elementsOnPage * _currentPage - 1;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _wordsCount = _keys.Length;

            var generated = -1;
            var languagesCount = _localizationData.Languages.Count;
            for (var i = 0; i < _keys.Length; i++)
            {
                if (!HasCheckSearchStr(i, languagesCount)) continue;

                generated++;
                if (generated < startWord || generated > endWord) continue;

                EditorGUILayout.BeginHorizontal();

                _keys[i] = EditorGUILayout.TextField(_keys[i]);
                for (var j = 0; j < languagesCount; j++)
                {
                    if (!ShowLanguages[j]) continue;
                    _translations[i, j] = EditorGUILayout.TextField(_translations[i, j]);
                }

                EditorGUILayout.EndHorizontal();
            }

            _generatedWordsCount = generated + 1;
            EditorGUILayout.EndScrollView();
        }

        private void DrawButtonMenu()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key")) AddKey();
            if (GUILayout.Button("Remove Last Key")) RemoveLastKey();
            if (GUILayout.Button("Up")) Up();
            if (GUILayout.Button("Down")) Down();
            if (GUILayout.Button("Save")) Save();
            EditorGUILayout.EndHorizontal();

            var saveElementsOnPage = _elementsOnPage;
            var saveCurrentPage = _currentPage;

            EditorGUILayout.BeginHorizontal();
            _elementsOnPage = EditorGUILayout.IntSlider(_elementsOnPage, 10, 100);
            _currentPage = EditorGUILayout.IntField(_currentPage);
            _pagesCount = _generatedWordsCount / _elementsOnPage;
            _pagesCount += _generatedWordsCount % _elementsOnPage == 0 ? 0 : 1;

            EditorGUILayout.LabelField($" / {_pagesCount}     (W:{_wordsCount} CW:{_generatedWordsCount})");
            if (GUILayout.Button("<"))
            {
                _currentPage--;
                if (_currentPage < 1) _currentPage = 1;
            }

            if (GUILayout.Button(">"))
            {
                _currentPage++;
                if (_currentPage > _pagesCount) _currentPage = _pagesCount;
            }

            EditorGUILayout.EndHorizontal();

            if (_currentPage != saveCurrentPage) EditorPrefs.SetInt(CurrentPagePref, _currentPage);

            if (_elementsOnPage != saveElementsOnPage) EditorPrefs.SetInt(WordsOnPagePref, _elementsOnPage);
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

            if (_keys[i].Contains(_searchText, StringComparison.Ordinal)) return true;

            for (var j = 0; j < languagesCount; j++)
            {
                if (_translations[i, j].Contains(_searchText, StringComparison.OrdinalIgnoreCase)) return true;
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
                "CSV URL";
            EditorGUILayout.LabelField(labelText, GUI.tooltip);

            _spreadsheetUrl = EditorGUILayout.TextField(_spreadsheetUrl);

            if (GUILayout.Button("Download CSV")) DownloadCsv();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHelpersElements()
        {
            var buttonOpenText = _isHelpersOpen ? "Close Helpers" : "Open Helpers";
            if (GUILayout.Button(buttonOpenText)) _isHelpersOpen = !_isHelpersOpen;
            if (!_isHelpersOpen) return;

            DrawCsvMenu();
            DrawGoogleSpreadSheetDownload();
            DrawGoogleLocalization();
            DrawKeyConverter();
            DrawChooseLanguages();
        }

        private void DrawGoogleLocalization()
        {
            _googleTextHeight = EditorGUILayout.IntSlider(
                "Text Height", _googleTextHeight, 20, 500);

            EditorGUILayout.BeginHorizontal();

            _googleFromLanguage = EditorGUILayout.Popup(_googleFromLanguage, GoogleTranslateLanguages);
            _googleToLanguage = EditorGUILayout.Popup(_googleToLanguage, GoogleTranslateLanguages);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            var width = _window.position.width / 2;
            _googleFromText = EditorGUILayout.TextArea(_googleFromText, _textAreaStyle,
                GUILayout.Height(_googleTextHeight), GUILayout.Width(width));
            _googleToText = EditorGUILayout.TextArea(_googleToText, _textAreaStyle,
                GUILayout.Height(_googleTextHeight), GUILayout.Width(width));

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Translate")) TranslateFromGoogle().Forget();
        }

        private async UniTask TranslateFromGoogle()
        {
            var fromLanguage = GoogleTranslateLanguages[_googleFromLanguage];
            var toLanguage = GoogleTranslateLanguages[_googleToLanguage];
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
                _googleToText = jsonArray[0]![0]![0]!.ToString();
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

        private void DrawKeyConverter()
        {
            EditorGUILayout.BeginHorizontal();
            _fromKeyConverter = EditorGUILayout.TextField(_fromKeyConverter);
            _toKeyConverter = EditorGUILayout.TextField(_toKeyConverter);
            if (GUILayout.Button("Convert to key")) _toKeyConverter = _fromKeyConverter.ToLower().Replace(" ", "_");

            EditorGUILayout.EndHorizontal();
        }

        private void DrawCsvMenu()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Import CSV")) ImportCsv();

            if (GUILayout.Button("Export CSV")) ExportCsv();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawChooseLanguages()
        {
            var showChooseLanguagesText = _showChooseLanguages ? "Close Languages" : "Open Languages";
            if (GUILayout.Button(showChooseLanguagesText)) _showChooseLanguages = !_showChooseLanguages;
            if (!_showChooseLanguages) return;
            var localizationsCount = _localizationData.Languages.Count;

            for (var i = 0; i < localizationsCount; i++)
            {
                var language = _localizationData.Languages[i];
                ShowLanguages[i] = EditorGUILayout.Toggle(language.SystemLanguage.ToString(), ShowLanguages[i]);
                EditorPrefs.SetBool($"show_in_window_language_{i}", ShowLanguages[i]);
            }
        }
    }
}
#endif