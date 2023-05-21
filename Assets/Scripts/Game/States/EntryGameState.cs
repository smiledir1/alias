using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Helper;

namespace Game.States
{
    public class EntryGameState : GameState
    {
        [Service]
        private static IAssetsService _assetsService;
        
        protected override async UniTask OnEnterState()
        {
#if DEV_ENV
            await UniTask.WhenAll(
                _assetsService.InstantiateAsync("CheatsButton"),
                _assetsService.InstantiateAsync("IngameDebugConsole"));
#endif

            var metaState = new MetaGameState();
            metaState.GoToState().Forget();
        }
    }
}