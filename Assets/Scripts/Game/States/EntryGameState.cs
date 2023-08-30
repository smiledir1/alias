using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Audio;
using Services.Helper;

namespace Game.States
{
    public class EntryGameState : GameState
    {
        [Service]
        private static IAssetsService _assetsService;
        
        [Service]
        private static IAudioService _audioService;

        protected override async UniTask OnEnterState()
        {
            _audioService.PlayMusic("main_music").Forget();
#if DEV_ENV
            await UniTask.WhenAll(
                _assetsService.InstantiateAsync("CheatsButton"),
                _assetsService.InstantiateAsync("IngameDebugConsole"));
#endif
        }
    }
}