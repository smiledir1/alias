using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Audio
{
    //TODO: сделать обертку над AssetReference для AseetService
    [Serializable]
    public class SoundSettings
    {
        [SerializeField]
        private AssetReferenceT<AudioClip> _reference;

        [SerializeField]
        private SoundType _type;

        [SerializeField]
        private string _id;

        internal SoundType Type => _type;
        internal AssetReferenceT<AudioClip> ClipReference => _reference;
        internal string Id => _id;

        public SoundSettings(
            AssetReferenceT<AudioClip> reference,
            SoundType type,
            string id)
        {
            _reference = reference;
            _type = type;
            _id = id;
        }

        internal void TrySetId(string id)
        {
            if (string.IsNullOrEmpty(_id)) _id = id;
        }
    }
}