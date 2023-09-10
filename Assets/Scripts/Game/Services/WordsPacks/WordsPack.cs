using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.WordsPacks
{
    [CreateAssetMenu(fileName = nameof(WordsPack), menuName = "WordsPacks/WordsPack")]
    public class WordsPack : ScriptableObject
    {
        public List<string> words;

        private void OnValidate()
        {
            var hash = new HashSet<string>();
            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (hash.Contains(word))
                    Debug.Log($"Const: {word} {i}");
                else
                    hash.Add(word);
            }
        }
    }
}