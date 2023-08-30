using Cysharp.Threading.Tasks;
using Services.Advertisement;
using Services.Audio;
using Services.Common;
using Services.YandexGames;
using UnityEngine.Events;

namespace Services.YandexAdvertisement
{
    public class YandexAdvertisementService : Service, IAdvertisementService
    {
        private readonly IYandexGamesService _yandexGamesService;
        private readonly IAudioService _audioService;
        
        public YandexAdvertisementService(
            IYandexGamesService yandexGamesService,
            IAudioService audioService)
        {
            _yandexGamesService = yandexGamesService;
            _audioService = audioService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_yandexGamesService);
        }

        public async UniTask ShowInterstitialAd()
        {
            _audioService.PauseMusic();
            await _yandexGamesService.ShowFullscreenAd();
            _audioService.ResumeMusic();
        }

        public async UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null)
        {
            _audioService.PauseMusic();
            await _yandexGamesService.ShowRewardedVideoAd(onRewardedCallback);
            _audioService.ResumeMusic();
        }
    }
}