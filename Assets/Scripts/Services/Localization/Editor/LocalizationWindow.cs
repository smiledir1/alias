﻿using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

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

        private static GUIStyle _centeredLabel;
        private static GUIStyle _searchStyle;
        private static LocalizationData _localizationData;

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

            EditorGUILayout.BeginHorizontal();

            _currentAddLanguage = (SystemLanguage) EditorGUILayout.EnumPopup("Language", _currentAddLanguage);

            _currentAddLocalizeLanguage = EditorGUILayout.TextField(_currentAddLocalizeLanguage);

            if (GUILayout.Button("Add"))
            {
                AddLanguage();
            }

            EditorGUILayout.EndHorizontal();

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

            DrawScrollView();

            DrawCheckLabel();

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

            serializeWindowObject.ApplyModifiedProperties();
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
            const char RowSeparator = ';';
            const char ColumnSeparator = '\n';

            //TODO: проверить порядок

            var path = $"{Application.dataPath}/loc.csv";
            var text = File.ReadAllText(path, Encoding.UTF8);

            var lines = text.Split(ColumnSeparator);
            var header = lines[0].Split(RowSeparator);

            _keys = new string[lines.Length - 1];
            _translations = new string[lines.Length - 1, header.Length - 1];
            for (var i = 1; i < lines.Length; ++i)
            {
                var items = lines[i].Split(RowSeparator);
                var pos = i - 1;
                _keys[pos] = items[0];
                for (var j = 1; j < items.Length; j++)
                {
                    _translations[pos, j - 1] = items[j];
                }
            }

            Debug.Log("Import complete");
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
    }
}