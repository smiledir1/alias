using Cysharp.Threading.Tasks;
using Services.UI.PopupService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Popups.NewGame
{
    public class NewGamePopup : Popup<NewGamePopupModel>
    {
        [SerializeField]
        private Button _choosePackButton;

        [SerializeField]
        private TextMeshProUGUI _packName;
        
        [SerializeField]
        private Slider _roundTime;

        [SerializeField]
        private Toggle _isUnlimitedTimeForLastWord;

        [SerializeField]
        private Button _startGameButton;

        // private Pack _choosePack;
        
        protected override UniTask OnOpenAsync()
        {
            return base.OnOpenAsync();
        }

        private void OnChoosePackButton()
        {
            // show choose pack popup, get pack
        }

        private void OnStartGameButton()
        {
            //check
            //startGame
        }
    }
}