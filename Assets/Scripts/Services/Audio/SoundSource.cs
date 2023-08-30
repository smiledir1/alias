using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.Audio
{
    // TODO: прокинуть IAssetService
    internal class SoundSource
    {
        private const float _fadeDuration = 1f;

        private readonly AudioSource _audio;
        private readonly SoundSettings _settings;

        private AudioClip _clip;
        private bool Loop => _settings?.Type == SoundType.Music;
        private bool _isLoadingClip;

        public SoundType Type => _settings.Type;
        public bool IsPlaying => _audio.isPlaying;

        internal SoundSource(SoundSettings settings, GameObject root)
        {
            _settings = settings;
            _audio = root.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.loop = Loop;
        }

        internal async UniTask PlayOneShot()
        {
            if (_audio.isPlaying) return;
            await LoadClip();
            if (_clip != null)
                _audio.PlayOneShot(_clip);
        }

        internal async UniTask Play()
        {
            if (_audio.isPlaying) return;
            await LoadClip();
            _audio.Play();
        }

        internal async UniTask PlayFadeIn(float volume)
        {
            _audio.volume = 0f;
            await Play();
            await Fade(0f, volume, _fadeDuration);
        }

        internal async UniTask PlayFadeOut(float volume)
        {
            if (_audio == null || !_audio.isPlaying) return;
            await Fade(volume, 0f, _fadeDuration);
            _audio.Stop();
        }

        internal void SetVolume(float volume)
        {
            _audio.volume = volume;
        }
        
        internal void Mute(bool mute)
        {
            _audio.mute = mute;
        }

        private async UniTask Fade(float from, float to, float duration)
        {
            var timeStart = Time.time;
            var progress = 0f;

            while (progress < 1f)
            {
                progress = (Time.time - timeStart) / duration;
                if (_audio != null)
                {
                    _audio.volume = Mathf.Lerp(from, to, progress);
                }

                await UniTask.Yield();
            }
        }

        private async UniTask LoadClip()
        {
            if (_clip == null)
            {
                if (_isLoadingClip) return;
                _isLoadingClip = true;
                _clip = _settings.ClipReference.Asset == null
                    ? await _settings.ClipReference.LoadAssetAsync()
                    : _settings.ClipReference.Asset as AudioClip;

                _audio.clip = _clip;
                _isLoadingClip = false;
            }
        }
    }
}