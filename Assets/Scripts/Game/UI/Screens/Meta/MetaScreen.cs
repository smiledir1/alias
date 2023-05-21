using Common.Extensions;
using Cysharp.Threading.Tasks;
using Services.UI;
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
            
        }
        
        private void OnSettingsButton()
        {
            
        }
    }
}