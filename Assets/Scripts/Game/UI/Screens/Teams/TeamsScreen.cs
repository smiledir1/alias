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
        private TextMeshProUGUI packName;

        [SerializeField]
        private TextMeshProUGUI roundSeconds;

        [SerializeField]
        private TextMeshProUGUI round;

        [SerializeField]
        private TeamItem teamItemTemplate;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Button startGame;

        private readonly List<TeamItem> _teamItems = new();

        protected override UniTask OnOpenAsync()
        {
            backButton.SetClickListener(OnBackButton);
            startGame.SetClickListener(OnStartGameButton);

            packName.text = Model.WordsPacksConfigItem.name;
            roundSeconds.text = Model.RoundTimeSeconds.ToString();
            round.text = Model.CurrentRound.ToString();

            CreateTeamItems();
            return base.OnOpenAsync();
        }

        private void CreateTeamItems()
        {
            teamItemTemplate.gameObject.SetActive(false);
            foreach (var teamItem in _teamItems)
            {
                Destroy(teamItem.gameObject);
            }

            _teamItems.Clear();

            var parent = teamItemTemplate.transform.parent;
            var currentRound = Model.CurrentRound - 1;
            var teamsCount = Model.Teams.Count;
            var roundTeam = currentRound % teamsCount;
            for (var i = 0; i < teamsCount; i++)
            {
                var team = Model.Teams[i];
                var isRoundTeam = i == roundTeam;
                var teamItem = Instantiate(teamItemTemplate, parent);
                teamItem.Initialize(team, isRoundTeam);
                teamItem.gameObject.SetActive(true);
                _teamItems.Add(teamItem);
            }
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