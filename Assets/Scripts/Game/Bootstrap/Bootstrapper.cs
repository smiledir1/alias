using System.Collections.Generic;
using Common.Extensions;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.GameConfig;
using Game.Services.WordsPacks;
using Game.States;
using Game.UserData;
using Services.Assets;
using Services.Audio;
using Services.Localization;
using Services.Locator;
using Services.Logger;
using Services.UI.PopupService;
using Services.UI.ScreenService;
using Services.UserData;
using Services.Vibration;
using UnityEngine;

namespace Game.Bootstrap
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
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
            var servicesInitializeTime = Time.realtimeSinceStartup - devInitStartTime;
#endif
            await ServiceLocator.StartServices();
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
                      $"Service Initialize Time: {servicesInitializeTime}\n" +
                      $"Service Start Time: {servicesStartTime}\n" +
                      $"Service Time: {servicesTime}");
#endif
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

            var userDataService = new UserDataService(new List<UserDataObject>()
            {
                new SettingsUserData(),
                new GameUserData()
            }, false);
            ServiceLocator.AddService<IUserDataService>(userDataService);

            var popupService = new PopupService(assetsService);
            ServiceLocator.AddService<IPopupService>(popupService);

            var screenService = new ScreenService(assetsService);
            ServiceLocator.AddService<IScreenService>(screenService);

            // TODO: SceneLoader (Curtains?)
            // TODO: HttpService? (Get, Post)
            // TODO: InApps? (UnityServices)
            // TODO: Analytics? (UnityServices)

            // TODO: test audio, test http, test recycle, test pool
            // TODO: Recycle list (https://github.com/MdIqubal/Recyclable-Scroll-Rect)
            // TODO: Monobehaviour pool

            // Game

            var gameConfigService = new GameConfigService(assetsService);
            ServiceLocator.AddService<IGameConfigService>(gameConfigService);
            
            var wordsPacksService = new WordsPacksService(assetsService);
            ServiceLocator.AddService<IWordsPacksService>(wordsPacksService);
        }
    }
}