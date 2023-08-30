using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.WordsPacks
{
    [CreateAssetMenu(fileName = nameof(WordsPack), menuName = "WordsPacks/WordsPack")]
    public class WordsPack : ScriptableObject
    {
        public List<string> Words;

        private void OnValidate()
        {
            var hash = new HashSet<string>();
            for (var i = 0; i < Words.Count; i++)
            {
                var word = Words[i];
                if (hash.Contains(word))
                {
                    Debug.Log($"Const: {word} {i}");
                }
                else
                {
                    hash.Add(word);
                }
            }
        }
    }
}