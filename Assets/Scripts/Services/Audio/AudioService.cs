using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;
using UnityEngine;

namespace Services.Audio
{
    public class AudioService : Service, IAudioService
    {
        #region Consts

        private const string _sfxVolumeKey = "SfxVolume";
        private const string _musicVolumeKey = "MusicVolume";

        #endregion

        private readonly IAssetsService _assetsService;
        private readonly bool _pauseUnfocus;

        private GameObject _soundsRootGameObject;
        private AudioConfig _config;
        private readonly Dictionary<string, List<SoundSource>> _sources = new();
        private string _currentMusicId;
        private float _sfxVolume = 1f;
        private float _musicVolume = 1f;

        public AudioService(IAssetsService assetsService, bool pauseUnfocus = true)
        {
            _assetsService = assetsService;
            _pauseUnfocus = pauseUnfocus;
        }

        #region Service

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);

            _soundsRootGameObject = new GameObject("Audio");
            _config = await _assetsService.LoadAsset<AudioConfig>();
            _sfxVolume = PlayerPrefs.GetFloat(_sfxVolumeKey, _sfxVolume);
            _musicVolume = PlayerPrefs.GetFloat(_musicVolumeKey, _musicVolume);

            if (_pauseUnfocus) Application.focusChanged += OnFocusChange;
        }

        protected override UniTask OnDispose()
        {
            if (_pauseUnfocus) Application.focusChanged -= OnFocusChange;
            return base.OnDispose();
        }

        #endregion

        #region IAudioService

        public float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = value;
                PlayerPrefs.SetFloat(_sfxVolumeKey, value);
                PlayerPrefs.Save();
                foreach (var sourceList in _sources.Values)
                {
                    foreach (var source in sourceList)
                    {
                        if (source.Type != SoundType.Sfx) continue;
                        source.SetVolume(value);
                    }
                }
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                PlayerPrefs.SetFloat(_musicVolumeKey, value);
                PlayerPrefs.Save();
                foreach (var sourceList in _sources.Values)
                {
                    foreach (var source in sourceList)
                    {
                        if (source.Type != SoundType.Music) continue;
                        source.SetVolume(value);
                    }
                }
            }
        }

        public async UniTask PlayMusic(string id)
        {
            var soundSource = GetSoundSource(id);
            if (soundSource == null) return;
            if (!string.IsNullOrEmpty(_currentMusicId)) await StopMusic(_currentMusicId);
            await soundSource.PlayFadeIn(MusicVolume);
            _currentMusicId = id;
        }

        public async UniTask StopMusic(string id)
        {
            if (_soundsRootGameObject == null) return;
            var soundSource = GetSoundSource(id);
            if (soundSource == null) return;
            await soundSource.PlayFadeOut(MusicVolume);
            _currentMusicId = null;
        }

        public void PauseMusic()
        {
            MuteMusic(true);
        }

        public void ResumeMusic()
        {
            MuteMusic(false);
        }

        public async UniTask PlaySound(string id, bool multiSound = false)
        {
            var soundSource = GetSoundSource(id, multiSound);
            if (soundSource == null) return;
            await soundSource.PlayOneShot();
        }

        #endregion

        #region Private Methods

        private SoundSource GetSoundSource(string id, bool multiSound = false)
        {
            _sources.TryGetValue(id, out var soundSourceList);
            if (soundSourceList == null)
            {
                soundSourceList = new List<SoundSource>();
                _sources.Add(id, soundSourceList);
            }

            if (multiSound)
            {
                foreach (var source in soundSourceList)
                {
                    if (!source.IsPlaying) return source;
                }
            }
            else
            {
                if (soundSourceList.Count > 0) return soundSourceList[0];
            }

            var settings = _config.GetSoundSetting(id);
            if (settings == null) return null;
            var soundSource = new SoundSource(settings, _soundsRootGameObject);
            soundSourceList.Add(soundSource);
            switch (soundSource.Type)
            {
                case SoundType.Music:
                    soundSource.SetVolume(MusicVolume);
                    break;
                case SoundType.Sfx:
                    soundSource.SetVolume(SfxVolume);
                    break;
            }

            return soundSource;
        }

        private void MuteMusic(bool mute)
        {
            foreach (var sourceList in _sources.Values)
            {
                foreach (var source in sourceList)
                {
                    if (source.Type != SoundType.Music) continue;
                    source.Mute(mute);
                }
            }
        }

        private void OnFocusChange(bool focus)
        {
            MuteMusic(!focus);
        }

        #endregion
    }
}