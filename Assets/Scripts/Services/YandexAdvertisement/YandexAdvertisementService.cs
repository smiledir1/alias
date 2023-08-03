using Cysharp.Threading.Tasks;
using Services.Advertisement;
using Services.Common;
using Services.YandexGames;
using UnityEngine.Events;

namespace Services.YandexAdvertisement
{
    public class YandexAdvertisementService : Service, IAdvertisementService
    {
        private readonly IYandexGamesService _yandexGamesService;
        
        public YandexAdvertisementService(IYandexGamesService yandexGamesService)
        {
            _yandexGamesService = yandexGamesService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_yandexGamesService);
        }

        public async UniTask ShowInterstitialAd()
        {
            await _yandexGamesService.ShowFullscreenAd();
        }

        public async UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null)
        {
            await _yandexGamesService.ShowRewardedVideoAd(onRewardedCallback);
        }
    }
}