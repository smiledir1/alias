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
        private Button languageButton;

        [SerializeField]
        private TextMeshProUGUI languageName;

        [Header("Audio")]
        [SerializeField]
        private Button soundButton;

        [SerializeField]
        private GameObject soundsOn;

        [SerializeField]
        private GameObject soundOff;

        [SerializeField]
        private Button musicButton;

        [SerializeField]
        private GameObject musicsOn;

        [SerializeField]
        private GameObject musicOff;

        [Header("Other")]
        [SerializeField]
        private TextMeshProUGUI version;

        [SerializeField]
        private Button rulesButton;

        [Service]
        private static IAudioService _audioService;

        [Service]
        private static ILocalizationService _localizationService;

        [Service]
        private static IPopupService _popupService;

        protected override UniTask OnOpenAsync()
        {
            languageButton.SetClickListener(OnLanguageButton);
            soundButton.SetClickListener(OnSoundButton);
            musicButton.SetClickListener(OnMusicButton);
            rulesButton.SetClickListener(OnRulesButton);

            languageName.text = _localizationService.CurrentLanguageLocalizeName;
            version.text = Application.version;

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
            soundsOn.SetActive(!isOn);
            soundOff.SetActive(isOn);
            _audioService.SfxVolume = isOn ? 0f : 1f;
        }

        private void OnMusicButton()
        {
            var isOn = _audioService.MusicVolume > 0.01f;
            musicsOn.SetActive(!isOn);
            musicOff.SetActive(isOn);
            _audioService.MusicVolume = isOn ? 0f : 1f;
        }

        private void CheckSounds()
        {
            var isOn = _audioService.SfxVolume > 0.01f;
            soundsOn.SetActive(isOn);
            soundOff.SetActive(!isOn);
        }

        private void CheckMusic()
        {
            var isOn = _audioService.MusicVolume > 0.01f;
            musicsOn.SetActive(isOn);
            musicOff.SetActive(!isOn);
        }

        private async UniTask ChangeLanguage()
        {
            await _localizationService.ChangeLanguage();
            var localizations = FindObjectsOfType<TMPLocalization>(true);
            languageName.text = _localizationService.CurrentLanguageLocalizeName;
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