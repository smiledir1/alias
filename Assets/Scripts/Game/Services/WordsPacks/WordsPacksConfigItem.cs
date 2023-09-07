using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Services.WordsPacks
{
    [Serializable]
    public class WordsPacksConfigItem
    {
        public string name;
        public string description;
        public string exampleWords;
        public string wordsCount;
        public SystemLanguage language;
        public AssetReferenceT<WordsPack> wordsPack;
    }
}