using System.Collections.Generic;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Teams
{
    public class TeamsScreen : UIObject<TeamsScreenModel>
    {
        [SerializeField]
        private TextMeshProUGUI _packName;

        [SerializeField]
        private TextMeshProUGUI _roundSeconds;

        [SerializeField]
        private TeamItem _teamItemTemplate;

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private Button _startGame;

        private List<TeamItem> _teamItems;

        protected override UniTask OnOpenAsync()
        {
            _backButton.SetListener(OnBackButton);
            _startGame.SetListener(OnStartGameButton);

            _packName.text = Model.WordsPacksConfigItem.Name;
            _roundSeconds.text = Model.RoundTimeSeconds.ToString();

            CreateTeamItems();
            return base.OnOpenAsync();
        }

        private void CreateTeamItems()
        {
            foreach (var teamItem in _teamItems)
            {
                Destroy(teamItem.gameObject);
            }

            _teamItems.Clear();

            var parent = _teamItemTemplate.transform.parent;
            var currentRound = Model.CurrentRound - 1;
            var teamsCount = Model.Teams.Count;
            var roundTeam = currentRound % teamsCount;
            for (var i = 0; i < teamsCount; i++)
            {
                var team = Model.Teams[i];
                var isRoundTeam = i == roundTeam;
                var teamItem = Instantiate(_teamItemTemplate, parent);
                teamItem.Initialize(team, isRoundTeam);
                teamItem.gameObject.SetActive(true);
                _teamItems.Add(teamItem);
            }
        }

        private void OnBackButton()
        {
            Model.GoBack();
            Close();
        }

        private void OnStartGameButton()
        {
            Model.StartRound();
            Close();
        }
    }
}