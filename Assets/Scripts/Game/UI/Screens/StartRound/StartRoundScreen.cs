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
        private TextMeshProUGUI teamName;

        [SerializeField]
        private TextMeshProUGUI teamScore;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Button startGame;

        protected override UniTask OnOpenAsync()
        {
            backButton.SetClickListener(OnBackButton);
            startGame.SetClickListener(OnStartGameButton);

            teamName.text = Model.RoundTeam.Name;
            teamScore.text = Model.RoundTeam.Score.ToString();

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