using System;
using UnityEngine.AddressableAssets;

namespace Game.Services.WordsPacks
{
    [Serializable]
    public class WordsPacksConfigItem
    {
        public string Name;
        public string Description;
        public string ExampleWords;
        public AssetReferenceT<WordsPack> WordsPack;
    }
}