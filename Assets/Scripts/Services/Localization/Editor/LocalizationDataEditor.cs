﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Services.Localization.Editor
{
    [CustomEditor(typeof(LocalizationData))]
    public class LocalizationDataEditor : UnityEditor.Editor
    {
        private LocalizationData _localizationData;

        private void OnEnable()
        {
            _localizationData = target as LocalizationData;
        }

        public override void OnInspectorGUI()
        {
            // if (GUILayout.Button("UpdateEntries")) DownloadAndParseTable().Forget();

            base.OnInspectorGUI();

            if (GUI.changed) Validate();
        }

        // private UniTask DownloadAndParseTable()
        // {
        //     // TODO: Test
        //     //TODO: Add ProgressBar
        //     // var parseData = await DownloadTable(_localizationData.SpreadsheetUrl);
        //     // UpdateEntries(parseData);
        // }

        // private static async UniTask<string> DownloadTable(string actualUrl)
        // {
        //     using var request = UnityWebRequest.Get(actualUrl);
        //     await request.SendWebRequest();
        //     if (request.result == UnityWebRequest.Result.ConnectionError ||
        //         request.result == UnityWebRequest.Result.ProtocolError ||
        //         request.result == UnityWebRequest.Result.DataProcessingError)
        //     {
        //         Debug.LogError(request.error);
        //         return string.Empty;
        //     }
        //
        //     Debug.Log("Successful download");
        //     return request.downloadHandler.text;
        // }
        //
        // private void UpdateEntries(string text)
        // {
        //     if (string.IsNullOrEmpty(text))
        //     {
        //         Debug.LogError("Wrong Parse Data");
        //         return;
        //     }
        //
        //     const char RowSeparator = ',';
        //     const char ColumnSeparator = '\n';
        //
        //     var languageEntries = new List<LanguageEntry>();
        //     var languageEntriesPoses = new List<int>();
        //     var lines = text.Split(ColumnSeparator);
        //     var header = lines[0].Split(RowSeparator);
        //     for (var i = 1; i < header.Length; ++i)
        //     {
        //         if (!Enum.TryParse(header[i], out SystemLanguage language)) continue;
        //         var entry = _localizationData.Languages.Find(
        //             x => x.SystemLanguage == language);
        //         entry.LanguageWords.editorAsset.Entries.Clear();
        //         languageEntries.Add(entry.LanguageWords.editorAsset);
        //         EditorUtility.SetDirty(entry.LanguageWords.editorAsset);
        //         languageEntriesPoses.Add(i);
        //     }
        //
        //     for (var i = 1; i < lines.Length; ++i)
        //     {
        //         var items = lines[i].Split(RowSeparator);
        //         for (var j = 0; i < languageEntries.Count; j++)
        //         {
        //             var languageEntry = languageEntries[j];
        //             var languagePos = languageEntriesPoses[j];
        //             var localizationEntry = new LocalizationEntry(items[0], items[languagePos]);
        //             languageEntry.Entries.Add(localizationEntry);
        //         }
        //
        //         for (var j = 1; j < languageEntries.Count + 1; j++)
        //         {
        //             var localizationEntry = new LocalizationEntry(items[0], items[j]);
        //             languageEntries[j - 1].Entries.Add(localizationEntry);
        //         }
        //     }
        // }

        private void Validate()
        {
            for (var i = 0; i < _localizationData.Languages.Count; i++)
            {
                var language = _localizationData.Languages[i];
                if (language?.LanguageWords == null || language.LanguageWords.editorAsset == null) continue;
                if (language.SystemLanguage != language.LanguageWords.editorAsset.Language)
                    Debug.LogError($"Different languages {i} {language.SystemLanguage}");
            }
        }
    }
}
#endif