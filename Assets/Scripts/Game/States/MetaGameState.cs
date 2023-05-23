using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.UI.Screens.Meta;
using Services.Helper;
using Services.UI.ScreenService;

namespace Game.States
{
    public class MetaGameState : GameState
    {
        [Service]
        private static IScreenService _screenService;

        protected override async UniTask OnEnterState()
        {
            var metaScreenModel = new MetaScreenModel();
            await _screenService.ShowAsync<MetaScreen>(metaScreenModel);
        }
    }
}
