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
        private bool _isJsonWords;
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
            _isJsonWords = EditorGUILayout.Toggle("Is Json Words", _isJsonWords);
            _words = EditorGUILayout.TextArea(_words, GUILayout.Height(100));

            if (GUILayout.Button("Create new pack")) CreateNewPack();
        }

        private void CreateNewPack()
        {
            var wordsPack = CreateInstance<WordsPack>();
            var wordsHash = new HashSet<string>();
            var wordsList = new List<string>();
            if (_isJsonWords)
            {
                var jsonWords = JsonUtility.FromJson<JsonWords>(_words);
                wordsList = jsonWords.words;
                //TODO: check dublicates
            }
            else
            {
                var words = _words.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    var clearWord = word.Replace("  ", "");
                    clearWord = clearWord.Replace('\r', '\0');
                    clearWord = clearWord.Replace('\n', '\0');
                    if (wordsHash.Contains(clearWord)) continue;
                    wordsHash.Add(clearWord);
                    wordsList.Add(clearWord);
                }
            }
           

            wordsPack.words = wordsList;

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
                name = _name,
                description = _description,
                exampleWords = WordsPacksConfig.GetExampleWords(wordsPack),
                wordsCount = wordsList.Count.ToString(),
                language = _packLanguage,
                wordsPack = new AssetReferenceT<WordsPack>(guid)
            };
            wordsPacksConfig.wordsPacksItems.Add(newConfigItem);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Serializable]
        private class JsonWords
        {
            public List<string> words;
        }
    }
    
    
}
#endif