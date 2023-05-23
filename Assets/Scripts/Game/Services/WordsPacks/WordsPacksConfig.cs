using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.WordsPacks
{
    [CreateAssetMenu(fileName = nameof(WordsPacksConfig), menuName = "WordsPacks/WordsPacksConfig")]
    public class WordsPacksConfig : ScriptableObject
    {
        public List<WordsPacksConfigItem> WordsPacksItems;

#if UNITY_EDITOR
        private const int ExampleWords = 10;
        
        [NaughtyAttributes.Button]
        private void CalculateWords()
        {
            foreach (var item in WordsPacksItems)
            {
                if (item.WordsPack == null)
                {
                    Debug.LogWarning("Empty pack ");
                    continue;
                }

                var wordsList = item.WordsPack.editorAsset.Words;
                var wordsCount = wordsList.Count > ExampleWords ? ExampleWords : wordsList.Count;
                item.ExampleWords = string.Empty;
                for (var i = 0; i < wordsCount; i++)
                {
                    var rndPos = Random.Range(0, wordsList.Count);
                    var word = wordsList[rndPos];
                    item.ExampleWords += $"{word}; ";
                }
            }
        }
#endif
    }
}