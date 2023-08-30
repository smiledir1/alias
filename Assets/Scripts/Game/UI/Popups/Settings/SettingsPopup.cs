using Common.Extensions;
using Cysharp.Threading.Tasks;
using Game.UI.Helper;
using Game.UI.Popups.Rules;
using Services.Audio;
using Services.Helper;
using Services.Localization;
using Services.Localization.Components;
using Services.UI.PopupService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Popups.Settings
{
    public class SettingsPopup : Popup<SettingsPopupModel>
    {
        [Header("Language")]
        [SerializeField]
        private Button _languageButton;

        [SerializeField]
        private TextMeshProUGUI _languageName;

        [Header("Audio")]
        [SerializeField]
        private Button _soundButton;

        [SerializeField]
        private GameObject _soundsOn;

        [SerializeField]
        private GameObject _soundOff;

        [SerializeField]
        private Button _musicButton;

        [SerializeField]
        private GameObject _musicsOn;

        [SerializeField]
        private GameObject _musicOff;

        [Header("Other")]
        [SerializeField]
        private TextMeshProUGUI _version;

        [SerializeField]
        private Button _rulesButton;

        [Service]
        private static IAudioService _audioService;

        [Service]
        private static ILocalizationService _localizationService;

        [Service]
        private static IPopupService _popupService;

        protected override UniTask OnOpenAsync()
        {
            _languageButton.SetClickListener(OnLanguageButton);
            _soundButton.SetClickListener(OnSoundButton);
            _musicButton.SetClickListener(OnMusicButton);
            _rulesButton.SetClickListener(OnRulesButton);
            
            _languageName.text = _localizationService.CurrentLanguageLocalizeName;
            _version.text = Application.version;

            CheckSounds();
            CheckMusic();
            return base.OnOpenAsync();
        }

        private void OnLanguageButton()
        {
            ChangeLanguage().Forget();
        }

        private void OnSoundButton()
        {
            var isOn = _audioService.SfxVolume > 0.01f;
            _soundsOn.SetActive(!isOn);
            _soundOff.SetActive(isOn);
            _audioService.SfxVolume = isOn ? 0f : 1f;
        }

        private void OnMusicButton()
        {
            var isOn = _audioService.MusicVolume > 0.01f;
            _musicsOn.SetActive(!isOn);
            _musicOff.SetActive(isOn);
            _audioService.MusicVolume = isOn ? 0f : 1f;
        }

        private void CheckSounds()
        {
            var isOn = _audioService.SfxVolume > 0.01f;
            _soundsOn.SetActive(isOn);
            _soundOff.SetActive(!isOn);
        }
        private void CheckMusic()
        {
            var isOn = _audioService.MusicVolume > 0.01f;
            _musicsOn.SetActive(isOn);
            _musicOff.SetActive(!isOn);
        }

        private async UniTask ChangeLanguage()
        {
            await _localizationService.ChangeLanguage();
            var localizations = FindObjectsOfType<TMPLocalization>(true);
            _languageName.text = _localizationService.CurrentLanguageLocalizeName;
            foreach (var localization in localizations)
            {
                localization.Recalculate();
            }
        }

        private void OnRulesButton()
        {
            var emptyModel = new EmptyUIModel();
            _popupService.ShowAsync<RulesPopup>(emptyModel).Forget();
        }
    }
}