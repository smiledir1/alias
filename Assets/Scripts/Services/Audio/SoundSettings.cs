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
        private AssetReferenceT<AudioClip> reference;

        [SerializeField]
        private SoundType type;

        [SerializeField]
        private string id;

        internal SoundType Type => type;
        internal AssetReferenceT<AudioClip> ClipReference => reference;
        internal string Id => id;

        public SoundSettings(
            AssetReferenceT<AudioClip> reference,
            SoundType type,
            string id)
        {
            this.reference = reference;
            this.type = type;
            this.id = id;
        }

        internal void TrySetId(string audioId)
        {
            if (string.IsNullOrEmpty(id)) id = audioId;
        }
    }
}