using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.WordsPacks
{
    [CreateAssetMenu(fileName = nameof(WordsPack), menuName = "WordsPacks/WordsPack")]
    public class WordsPack : ScriptableObject
    {
        public List<string> words;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var deleteNums = new List<int>();
            var hash = new HashSet<string>();
            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (hash.Contains(word))
                {
                    Debug.Log($"Const: {word} {i} {name}");
                    deleteNums.Add(i);
                }
                else
                {
                    hash.Add(word);
                }
            }

            for (var i = deleteNums.Count - 1; i >= 0; i--)
            {
                var num = deleteNums[i];
                words.RemoveAt(num);
            }

            if (deleteNums.Count > 0)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}