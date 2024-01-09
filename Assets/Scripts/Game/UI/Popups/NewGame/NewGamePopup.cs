using System.Collections.Generic;
using Common.Extensions;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.States;
using Game.UI.Popups.ChoosePack;
using Game.UI.Popups.Message;
using Game.UI.Popups.Teams;
using Services.Helper;
using Services.UI.PopupService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Popups.NewGame
{
    public class NewGamePopup : Popup<NewGamePopupModel>
    {
        [SerializeField]
        private Button choosePackButton;

        [SerializeField]
        private TextMeshProUGUI packName;

        [SerializeField]
        private Slider roundTime;

        [SerializeField]
        private Toggle isUnlimitedTimeForLastWord;

        [SerializeField]
        private Toggle freeSkipToggle;

        [SerializeField]
        private Button addTeamButton;

        [SerializeField]
        private Button removeTeamButton;

        [SerializeField]
        private TeamItem teamItemTemplate;

        [SerializeField]
        private Button startGameButton;

        [Service]
        private static IPopupService _popupService;

        private WordsPacksConfigItem _choosePack;
        private readonly List<Team> _teams = new();
        private readonly List<TeamItem> _teamItems = new();

        protected override UniTask OnOpenAsync()
        {
            choosePackButton.SetClickListener(() => OnChoosePackButton().SafeForget());
            addTeamButton.SetClickListener(OnAddTeamButton);
            removeTeamButton.SetClickListener(OnRemoveTeamButton);
            startGameButton.SetClickListener(OnStartGameButton);
            ClearAll();
            return base.OnOpenAsync();
        }

        private async UniTask OnChoosePackButton()
        {
            var choosePackPopupModel = new ChoosePackPopupModel();
            _popupService.ShowAsync<ChoosePackPopup>(choosePackPopupModel).Forget();
            _choosePack = await choosePackPopupModel.WaitForPackItem();
            packName.text = _choosePack.name;
        }

        private void OnAddTeamButton()
        {
            AddTeamFromPopup().Forget();
            // var team = new Team($"Team {_teams.Count}", null);
            // _teams.Add(team);
            // var teamsPopupModel = new TeamsPopupModel();
            // _popupService.ShowAsync<TeamsPopup>(teamsPopupModel);
            // await teamsPopupModel.WaitForTeam();
            // CreateTeamItem(team);
        }

        private async UniTask AddTeamFromPopup()
        {
            var teamsPopupModel = new TeamsPopupModel(_teams);
            _popupService.ShowAsync<TeamsPopup>(teamsPopupModel).Forget();
            var team = await teamsPopupModel.WaitForTeam();
            CreateTeamItem(team);
        }

        private void OnRemoveTeamButton()
        {
            if (_teams.Count == 0) return;
            _teams.RemoveAt(_teams.Count - 1);
            var teamItem = _teamItems[^1];
            Destroy(teamItem.gameObject);
            _teamItems.RemoveAt(_teamItems.Count - 1);
        }

        private void OnStartGameButton()
        {
            if (!CheckStartGame()) return;
            Close();
            var roundTimeSeconds = (int) roundTime.value;
            var curIsUnlimitedTimeForLastWord = isUnlimitedTimeForLastWord.isOn;
            var isFreeSkip = freeSkipToggle.isOn;

            var inGameState = new InGameState(
                _choosePack,
                roundTimeSeconds,
                curIsUnlimitedTimeForLastWord,
                !isFreeSkip,
                _teams);
            inGameState.GoToState().SafeForget();
        }

        private bool CheckStartGame()
        {
            if (_choosePack == null)
            {
                var messagePopupModel = new MessagePopupModel(
                    "start_game_error",
                    "pack_not_selected",
                    true);
                _popupService.ShowAsync<MessagePopup>(messagePopupModel);
                return false;
            }

            if (_teams.Count < 2)
            {
                var messagePopupModel = new MessagePopupModel(
                    "start_game_error",
                    "multiple_teams_must_participate",
                    true);
                _popupService.ShowAsync<MessagePopup>(messagePopupModel);
                return false;
            }

            // if (_roundTime.value < 5)
            // {
            //     var messagePopupModel = new MessagePopupModel(
            //         "Start Game Error", 
            //         "round time < 5");
            //     _popupService.ShowAsync<MessagePopup>(messagePopupModel);
            //     return false;
            // }
            return true;
        }

        private void CreateTeamItem(Team team)
        {
            var parent = teamItemTemplate.transform.parent;
            var teamItem = Instantiate(teamItemTemplate, parent);
            teamItem.Initialize(team, OnRemoveTeamItem);
            teamItem.gameObject.SetActive(true);
            _teamItems.Add(teamItem);
            _teams.Add(team);
        }

        private void OnRemoveTeamItem(TeamItem teamItem)
        {
            var index = _teamItems.IndexOf(teamItem);
            _teamItems.RemoveAt(index);
            _teams.RemoveAt(index);
            Destroy(teamItem.gameObject);
        }

        private void ClearAll()
        {
            teamItemTemplate.gameObject.SetActive(false);
            foreach (var teamItem in _teamItems)
            {
                Destroy(teamItem.gameObject);
            }

            _teamItems.Clear();
            _teams.Clear();
        }
    }
}