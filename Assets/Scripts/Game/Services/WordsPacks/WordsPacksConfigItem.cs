using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Services.WordsPacks
{
    [Serializable]
    public class WordsPacksConfigItem
    {
        public string Name;
        public string Description;
        public string ExampleWords;
        public string WordsCount;
        public SystemLanguage Language;
        public AssetReferenceT<WordsPack> WordsPack;
    }
}