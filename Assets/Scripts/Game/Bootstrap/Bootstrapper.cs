using System.Collections.Generic;
using Common.Extensions;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.GameConfig;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.States;
using Game.UserData.Game;
using Services.Advertisement;
using Services.Analytics;
using Services.Appodeal;
using Services.Assets;
using Services.Audio;
using Services.Device;
using Services.Localization;
using Services.Locator;
using Services.Logger;
using Services.UI.PopupService;
using Services.UI.ScreenService;
using Services.UserData;
using Services.Vibration;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using Services.UserData.WebGL;
#else
using Services.UserData.File;
#endif

#if YANDEX_PLATFORM
using Services.YandexAdvertisement;
using Services.YandexGames;
#endif

namespace Game.Bootstrap
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
#if DEV_ENV
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0) return;
#endif
            InitializeAsync().SafeForget();
        }

        private static async UniTask InitializeAsync()
        {
#if DEV_ENV
            var devInitStartTime = Time.realtimeSinceStartup;
#endif

            RegisterServices();
            await ServiceLocator.InitializeServices();
#if DEV_ENV
            var servicesStartTime = Time.realtimeSinceStartup - devInitStartTime;
#endif
            ServiceLocator.FillServices();

#if DEV_ENV
            var servicesTime = Time.realtimeSinceStartup - devInitStartTime;
#endif

            var entryGameState = new EntryGameState();
            await entryGameState.GoToState();

            var metaState = new MetaGameState();
            metaState.GoToState().Forget();

#if DEV_ENV
            var fullInitTime = Time.realtimeSinceStartup - devInitStartTime;
            Debug.Log($"InitializeTime: {fullInitTime}\n" +
                      // $"Service Initialize Time: {servicesInitializeTime}\n" +
                      $"Service Start Time: {servicesStartTime}\n" +
                      $"Service Time: {servicesTime}");
#endif
            Application.quitting += () => { ServiceLocator.DisposeServices().Forget(); };
        }

        private static void RegisterServices()
        {
            // Main

            var assetsService = new AssetsService();
            ServiceLocator.AddService<IAssetsService>(assetsService);

            var loggerService = new LoggerService();
            ServiceLocator.AddService<ILoggerService>(loggerService);

            var vibrationService = new VibrationService();
            ServiceLocator.AddService<IVibrationService>(vibrationService);

            var audioService = new AudioService(assetsService);
            ServiceLocator.AddService<IAudioService>(audioService);

            var localizationService = new LocalizationService(assetsService);
            ServiceLocator.AddService<ILocalizationService>(localizationService);

            var userDataObjects = new List<UserDataObject>
            {
                new GameUserData()
            };

#if UNITY_WEBGL && !UNITY_EDITOR
            var userDataService = new WebGLUserDataService(userDataObjects, false);
#else
            var userDataService = new FileUserDataService(userDataObjects, false);
#endif
            ServiceLocator.AddService<IUserDataService>(userDataService);

            var popupService = new PopupService(assetsService);
            ServiceLocator.AddService<IPopupService>(popupService);

            var screenService = new ScreenService(assetsService);
            ServiceLocator.AddService<IScreenService>(screenService);

#if DEV_ENV
            var analyticsService = new FakeAnalyticsService();
#else
            var analyticsService = new UniversalAnalyticsService(assetsService);
#endif
            ServiceLocator.AddService<IAnalyticsService>(analyticsService);

#if YANDEX_PLATFORM
            var yandexService = new YandexGamesService(assetsService);
            ServiceLocator.AddService<IYandexGamesService>(yandexService);

            var yandexAdvertisementService = new YandexAdvertisementService(yandexService, audioService);
            ServiceLocator.AddService<IAdvertisementService>(yandexAdvertisementService);
#endif

#if UNITY_ANDROID || UNITY_IOS
            var appodealService = new AppodealService(assetsService);
            ServiceLocator.AddService<IAdvertisementService>(appodealService);
#endif
            
            var deviceService = new DeviceService();
            ServiceLocator.AddService<IDeviceService>(deviceService);

            // Game

            var gameConfigService = new GameConfigService(assetsService);
            ServiceLocator.AddService<IGameConfigService>(gameConfigService);

            var wordsPacksService = new WordsPacksService(assetsService);
            ServiceLocator.AddService<IWordsPacksService>(wordsPacksService);

            var teamsService = new TeamsService(assetsService, localizationService);
            ServiceLocator.AddService<ITeamsService>(teamsService);
        }
    }
}