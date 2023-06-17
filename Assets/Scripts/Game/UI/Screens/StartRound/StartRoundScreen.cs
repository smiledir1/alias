using Common.Extensions;
using Cysharp.Threading.Tasks;
using Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.StartRound
{
    public class StartRoundScreen : UIObject<StartRoundScreenModel>
    {
        [SerializeField]
        private TextMeshProUGUI _teamName;

        [SerializeField]
        private TextMeshProUGUI _teamScore;

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private Button _startGame;

        protected override UniTask OnOpenAsync()
        {
            _backButton.SetClickListener(OnBackButton);
            _startGame.SetClickListener(OnStartGameButton);

            _teamName.text = Model.RoundTeam.Name;
            _teamScore.text = Model.RoundTeam.Score.ToString();

            return base.OnOpenAsync();
        }
        
        private void OnBackButton()
        {
            Close();
            Model.GoBack();
        }

        private void OnStartGameButton()
        {
            Close();
            Model.StartRound();
        }
    }
}