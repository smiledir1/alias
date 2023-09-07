using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.WordsPacks
{
    [CreateAssetMenu(fileName = nameof(WordsPacksConfig), menuName = "WordsPacks/WordsPacksConfig")]
    public class WordsPacksConfig : ScriptableObject
    {
        public List<WordsPacksConfigItem> wordsPacksItems;
        
        private void OnEnable()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
#if UNITY_EDITOR
        private const int ExampleWords = 5;

        [NaughtyAttributes.Button]
        private void CalculateWords()
        {
            foreach (var item in wordsPacksItems)
            {
                if (item.WordsPack == null)
                {
                    Debug.LogWarning("Empty pack ");
                    continue;
                }

                item.ExampleWords = GetExampleWords(item.WordsPack.editorAsset);
                item.WordsCount = item.WordsPack.editorAsset.words.Count.ToString();
            }
        }

        public static string GetExampleWords(WordsPack wordsPack)
        {
            var usedIndexes = new HashSet<int>(ExampleWords);
            var wordsList = wordsPack.words;
            var wordsCount = wordsList.Count > ExampleWords ? ExampleWords : wordsList.Count;
            var exampleWords = string.Empty;
            for (var i = 0; i < wordsCount; i++)
            {
                int rndPos;
                do
                {
                    rndPos = Random.Range(0, wordsList.Count);
                } while (usedIndexes.Contains(rndPos));

                var word = wordsList[rndPos];
                if (word.EndsWith('\r')) word = word[..^1];
                if (word.EndsWith('\n')) word = word[..^1];
                exampleWords += $"{word}; ";
                usedIndexes.Add(rndPos);
            }

            return exampleWords;
        }
#endif
    }
}