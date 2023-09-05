using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioConfig), menuName = "Services/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField]
        private List<SoundSettings> _sounds;

        internal SoundSettings GetSoundSetting(string id)
        {
            return _sounds.Find(sound =>
                string.Compare(sound.Id, id, StringComparison.OrdinalIgnoreCase) == 0);
        }

#if UNITY_EDITOR
        public List<SoundSettings> Sounds => _sounds;

        public void AddSoundSettings(SoundSettings soundSettings)
        {
            for (var i = _sounds.Count - 1; i >= 0; i--)
            {
                var sound = _sounds[i];
                if (sound.ClipReference != null) continue;
                _sounds.RemoveAt(i);
            }

            if (_sounds.Exists(
                    s =>
                        s.ClipReference.editorAsset == soundSettings.ClipReference.editorAsset)) return;
            _sounds.Add(soundSettings);
        }

        private void OnValidate()
        {
            foreach (var sound in _sounds)
            {
                if (string.IsNullOrEmpty(sound.Id) &&
                    sound.ClipReference != null &&
                    sound.ClipReference.editorAsset != null)
                    sound.TrySetId(sound.ClipReference.editorAsset.name);
            }
        }
#endif
    }
}