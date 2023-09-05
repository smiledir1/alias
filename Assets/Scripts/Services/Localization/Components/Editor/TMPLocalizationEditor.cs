#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Services.Localization.Components.Editor
{
    [CustomEditor(typeof(TMPLocalization))]
    public class TMPLocalizationEditor : UnityEditor.Editor
    {
        private const string PathToConfigs = "Assets/Configs";
        private const int DrawCount = 10;
        private const SystemLanguage StripLanguage = SystemLanguage.English;

        private TMPLocalization _tmpLocalization;
        private LocalizationData _localizationData;
        private readonly HashSet<string> _localizationEntryKeys = new();
        private SearchKeyParameter _searchParameter;
        private bool _fullLanguagesDraw;
        private bool _notFoundKeys;
        private readonly List<string> _findKeys = new();
        private readonly List<string> _findTexts = new();

        private void OnEnable()
        {
            _tmpLocalization = target as TMPLocalization;

            var assetsFilter = $"t:{nameof(LocalizationData)}";
            var searchFolder = new[] {PathToConfigs};
            var assets = AssetDatabase.FindAssets(assetsFilter, searchFolder);
            if (assets.Length == 0)
            {
                Debug.LogError($"Create {nameof(LocalizationData)} First");
                return;
            }

            var configGuid = assets[0];
            var configPath = AssetDatabase.GUIDToAssetPath(configGuid);
            _localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(configPath);
            var englishEntry = _localizationData.Languages.Find(
                x => x.SystemLanguage == SystemLanguage.English);
            if (englishEntry == null)
            {
                Debug.LogError($"Create English Language");
                return;
            }

            _localizationEntryKeys.Clear();
            foreach (var localizationEntry in englishEntry.LanguageWords.editorAsset.Entries)
            {
                _localizationEntryKeys.Add(localizationEntry.Key);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLocalizationHelper();
        }

        private void DrawLocalizationHelper()
        {
            GUILayout.Space(10);
            GUILayout.Label("Helper");

            _searchParameter = (SearchKeyParameter) EditorGUILayout.EnumPopup(
                "Search Parameter", _searchParameter);

            var componentKey = _tmpLocalization.Key;
            if (string.IsNullOrEmpty(componentKey)) return;

            if (GUI.changed)
            {
                _findKeys.Clear();
                _findTexts.Clear();
            }

            if (!_localizationEntryKeys.Contains(componentKey))
            {
                DrawContainsKey(componentKey);
                return;
            }

            DrawUnContainsKey(componentKey);
        }

        private void DrawContainsKey(string componentKey)
        {
            if (GUI.changed)
            {
                _findKeys.Clear();
                EditorUtility.SetDirty(_tmpLocalization);
            }

            GUILayout.Label("Нажми, чтобы выбрать", EditorStyles.boldLabel);
            if (_findKeys.Count == 0)
            {
                var drawCount = 0;
                foreach (var entryKey in _localizationEntryKeys)
                {
                    var hasParameter = _searchParameter switch
                    {
                        SearchKeyParameter.Start => entryKey.StartsWith(componentKey, StringComparison.Ordinal),
                        SearchKeyParameter.Contains => entryKey.Contains(componentKey, StringComparison.Ordinal),
                        _ => false
                    };

                    if (hasParameter)
                    {
                        _findKeys.Add(entryKey);
                        if (GUILayout.Button(entryKey, "CN CountBadge"))
                        {
                            GUI.FocusControl(null);
                            _tmpLocalization.Key = entryKey;
                            EditorUtility.SetDirty(_tmpLocalization);
                        }

                        drawCount++;
                    }

                    if (drawCount >= DrawCount) break;
                }

                _notFoundKeys = drawCount == 0;
            }
            else
            {
                foreach (var findKey in _findKeys)
                {
                    if (GUILayout.Button(findKey, "CN CountBadge"))
                    {
                        GUI.FocusControl(null);
                        _tmpLocalization.Key = findKey;
                        EditorUtility.SetDirty(_tmpLocalization);
                    }
                }
            }

            if (_findKeys.Count >= DrawCount) GUILayout.Label("Еще..");

            if (_notFoundKeys) GUILayout.Box("Не найдено");
        }

        private void DrawUnContainsKey(string componentKey)
        {
            if (GUI.changed)
            {
                _findTexts.Clear();
                EditorUtility.SetDirty(_tmpLocalization);
            }

            EditorGUILayout.LabelField("Нажми на текст, чтобы посмотреть", EditorStyles.boldLabel);

            if (_fullLanguagesDraw)
            {
                if (_findTexts.Count == 0)
                {
                    foreach (var language in _localizationData.Languages)
                    {
                        var localizationEntry = language.LanguageWords.editorAsset.Entries.Find(
                            x => x.Key == componentKey);
                        DrawLocalizationText(localizationEntry.Text, language.SystemLanguage.ToString());
                        _findTexts.Add(localizationEntry.Text);
                    }
                }
                else
                {
                    var i = 0;
                    foreach (var language in _localizationData.Languages)
                    {
                        DrawLocalizationText(_findTexts[i], language.SystemLanguage.ToString());
                        i++;
                    }
                }

                GUILayout.Space(5);
                if (GUILayout.Button("Скрыть языки"))
                {
                    _fullLanguagesDraw = false;
                    _findTexts.Clear();
                }
            }
            else
            {
                if (_findTexts.Count == 0)
                {
                    var languageEntry = _localizationData.Languages.Find(
                        x => x.SystemLanguage == StripLanguage);

                    var localizationEntry = languageEntry.LanguageWords.editorAsset.Entries.Find(
                        x => x.Key == componentKey);
                    _findTexts.Add(localizationEntry.Text);
                    DrawLocalizationText(localizationEntry.Text, StripLanguage.ToString());
                }
                else
                    DrawLocalizationText(_findTexts[0], StripLanguage.ToString());

                GUILayout.Space(5);
                if (GUILayout.Button("Все языки"))
                {
                    _fullLanguagesDraw = true;
                    _findTexts.Clear();
                }
            }
        }

        private void DrawLocalizationText(string text, string language)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(language);
            if (GUILayout.Button(text, "Box"))
            {
                _tmpLocalization.Text.text = text;
                EditorUtility.SetDirty(_tmpLocalization);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public enum SearchKeyParameter
        {
            Start,
            Contains
        }
    }
}
#endif