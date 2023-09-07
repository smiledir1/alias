using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioConfig), menuName = "Services/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField]
        private List<SoundSettings> sounds;
        
        internal SoundSettings GetSoundSetting(string id)
        {
            return sounds.Find(sound =>
                string.Compare(sound.Id, id, StringComparison.OrdinalIgnoreCase) == 0);
        }

#if UNITY_EDITOR
        public List<SoundSettings> Sounds => sounds;

        public void AddSoundSettings(SoundSettings soundSettings)
        {
            for (var i = sounds.Count - 1; i >= 0; i--)
            {
                var sound = sounds[i];
                if (sound.ClipReference != null) continue;
                sounds.RemoveAt(i);
            }

            if (sounds.Exists(
                    s =>
                        s.ClipReference.editorAsset == soundSettings.ClipReference.editorAsset)) return;
            sounds.Add(soundSettings);
        }

        private void OnValidate()
        {
            foreach (var sound in sounds)
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