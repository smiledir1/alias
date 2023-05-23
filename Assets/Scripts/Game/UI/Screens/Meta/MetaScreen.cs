using Common.Extensions;
using Cysharp.Threading.Tasks;
using Game.UI.Popups.NewGame;
using Services.Helper;
using Services.UI;
using Services.UI.PopupService;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Meta
{
    public class MetaScreen : UIObject<MetaScreenModel>
    {
        [SerializeField]
        private Button _continueGameButton;
        
        [SerializeField]
        private Button _newGameButton;
        
        [SerializeField]
        private Button _settingsButton;

        [Service]
        private static IPopupService _popupService;

        protected override UniTask OnStartAsync()
        {
            _continueGameButton.SetListener(OnContinueGameButton);
            _newGameButton.SetListener(OnNewGameButton);
            _settingsButton.SetListener(OnSettingsButton);
            return base.OnStartAsync();
        }

        private void OnContinueGameButton()
        {
            
        }
        
        private void OnNewGameButton()
        {
            var newGamePopupModel = new NewGamePopupModel();
            _popupService.ShowAsync<NewGamePopup>(newGamePopupModel);
        }
        
        private void OnSettingsButton()
        {
            
        }
    }
}