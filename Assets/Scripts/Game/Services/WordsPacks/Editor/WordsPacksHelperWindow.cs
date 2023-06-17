#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Services.WordsPacks.Editor
{
    public class WordsPacksHelperWindow : EditorWindow
    {
        private static WordsPacksHelperWindow _window;

        private string _fileName;
        private string _name;
        private string _description;
        private string _words;
        private SystemLanguage _packLanguage;

        [MenuItem("Tools/Packs Window")]
        private static void InitializeWindow()
        {
            _window = GetWindow<WordsPacksHelperWindow>();
            _window.Show();
        }

        private void OnGUI()
        {
            if (_window == null) InitializeWindow();

            EditorGUILayout.LabelField("Pack Info:");
            
            EditorGUILayout.BeginHorizontal();
            _packLanguage = (SystemLanguage) EditorGUILayout.EnumPopup("Language", _packLanguage);
            if (GUILayout.Button("Russian")) _packLanguage = SystemLanguage.Russian;
            if (GUILayout.Button("English")) _packLanguage = SystemLanguage.English;
            EditorGUILayout.EndHorizontal();
            
            _fileName = EditorGUILayout.TextField("File Name", _fileName);
            _name = EditorGUILayout.TextField("Name", _name);

            EditorGUILayout.LabelField("Description");
            _description = EditorGUILayout.TextArea(_description, GUILayout.Height(100));

            EditorGUILayout.LabelField("Words");
            _words = EditorGUILayout.TextArea(_words, GUILayout.Height(100));

            if (GUILayout.Button("Create new pack"))
            {
                CreateNewPack();
            }
        }

        private void CreateNewPack()
        {
            var wordsPack = CreateInstance<WordsPack>();
            var words = _words.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var wordsHash = new HashSet<string>();
            var wordsList = new List<string>();
            foreach (var word in words)
            {
                var clearWord = word.Replace("  ", "");
                if(wordsHash.Contains(clearWord)) continue;
                wordsHash.Add(clearWord);
                wordsList.Add(clearWord);
            }
            wordsPack.Words = wordsList;
            
            var path = $"Assets/Configs/WordsPacks/{_fileName}.asset";
            AssetDatabase.CreateAsset(wordsPack, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = wordsPack;
            var guid = AssetDatabase.AssetPathToGUID(path);

            var wordsPacksConfigPath = "Assets/Configs/WordsPacks/WordsPacksConfig.asset";
            var wordsPacksConfig = AssetDatabase.LoadAssetAtPath<WordsPacksConfig>(wordsPacksConfigPath);
            var newConfigItem = new WordsPacksConfigItem
            {
                Name = _name,
                Description = _description,
                ExampleWords = WordsPacksConfig.GetExampleWords(wordsPack),
                WordsCount = words.Length.ToString(),
                Language = _packLanguage,
                WordsPack = new AssetReferenceT<WordsPack>(guid)
            };
            wordsPacksConfig.WordsPacksItems.Add(newConfigItem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
    }
}
#endif