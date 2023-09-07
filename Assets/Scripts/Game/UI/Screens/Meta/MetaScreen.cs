using Common.Extensions;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.States;
using Game.UI.Popups.NewGame;
using Game.UI.Popups.Settings;
using Game.UserData.Game;
using Services.Helper;
using Services.UI;
using Services.UI.PopupService;
using Services.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Meta
{
    public class MetaScreen : UIObject<MetaScreenModel>
    {
        [SerializeField]
        private Button continueGameButton;

        [SerializeField]
        private Button newGameButton;

        [SerializeField]
        private Button settingsButton;

        [Service]
        private static IPopupService _popupService;

        [Service]
        private static IUserDataService _userData;

        protected override UniTask OnOpenAsync()
        {
            var activeContinueButton = _userData.GetData<GameUserData>().currentRound != 0;
            continueGameButton.gameObject.SetActive(activeContinueButton);

            continueGameButton.SetClickListener(OnContinueGameButton);
            newGameButton.SetClickListener(OnNewGameButton);
            settingsButton.SetClickListener(OnSettingsButton);
            return base.OnStartAsync();
        }

        private void OnContinueGameButton()
        {
            var inGameState = new InGameState();
            inGameState.GoToState().SafeForget();
        }

        private void OnNewGameButton()
        {
            var newGamePopupModel = new NewGamePopupModel();
            _popupService.ShowAsync<NewGamePopup>(newGamePopupModel);
        }

        private void OnSettingsButton()
        {
            var settingsPopupModel = new SettingsPopupModel();
            _popupService.ShowAsync<SettingsPopup>(settingsPopupModel);
        }
    }
}