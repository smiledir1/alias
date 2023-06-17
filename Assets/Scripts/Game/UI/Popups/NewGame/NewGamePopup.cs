using System.Collections.Generic;
using Common.Extensions;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.States;
using Game.UI.Popups.ChoosePack;
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
        private Button _choosePackButton;

        [SerializeField]
        private TextMeshProUGUI _packName;

        [SerializeField]
        private Slider _roundTime;

        [SerializeField]
        private Toggle _isUnlimitedTimeForLastWord;

        [SerializeField]
        private Button _addTeamButton;

        [SerializeField]
        private Button _removeTeamButton;

        [SerializeField]
        private TeamItem _teamItemTemplate;

        [SerializeField]
        private Button _startGameButton;

        [Service]
        private static IPopupService _popupService;

        private WordsPacksConfigItem _choosePack;
        private readonly List<Team> _teams = new();
        private readonly List<TeamItem> _teamItems = new();

        protected override UniTask OnOpenAsync()
        {
            _choosePackButton.SetClickListener(() => OnChoosePackButton().SafeForget());
            _addTeamButton.SetClickListener(OnAddTeamButton);
            _removeTeamButton.SetClickListener(OnRemoveTeamButton);
            _startGameButton.SetClickListener(OnStartGameButton);
            ClearAll();
            return base.OnOpenAsync();
        }

        private async UniTask OnChoosePackButton()
        {
            var choosePackPopupModel = new ChoosePackPopupModel();
            _popupService.ShowAsync<ChoosePackPopup>(choosePackPopupModel).Forget();
            _choosePack = await choosePackPopupModel.WaitForPackItem();
            _packName.text = _choosePack.Name;
        }

        private void OnAddTeamButton()
        {
            var team = new Team($"Team {_teams.Count}");
            _teams.Add(team);
            CreateTeamItem(team);
        }

        private void OnRemoveTeamButton()
        {
            if(_teams.Count == 0) return;
            _teams.RemoveAt(_teams.Count - 1);
            var teamItem = _teamItems[^1];
            Destroy(teamItem.gameObject);
            _teamItems.RemoveAt(_teamItems.Count - 1);
        }

        private void OnStartGameButton()
        {
            if (!CheckStartGame()) return;
            Close();
            var roundTimeSeconds = (int) _roundTime.value;
            var isUnlimitedTimeForLastWord = _isUnlimitedTimeForLastWord.isOn;

            var inGameState = new InGameState(
                _choosePack,
                roundTimeSeconds,
                isUnlimitedTimeForLastWord,
                _teams);
            inGameState.GoToState().SafeForget();
        }

        private bool CheckStartGame()
        {
            if (_choosePack == null) return false;
            if (_teams.Count < 2) return false;
            if (_roundTime.value < 5) return false;
            return true;
        }

        private void CreateTeamItem(Team team)
        {
            var parent = _teamItemTemplate.transform.parent;
            var teamItem = Instantiate(_teamItemTemplate, parent);
            teamItem.Initialize(team);
            teamItem.gameObject.SetActive(true);
            _teamItems.Add(teamItem);
        }

        private void ClearAll()
        {
            _teamItemTemplate.gameObject.SetActive(false);
            foreach (var teamItem in _teamItems)
            {
                Destroy(teamItem.gameObject);
            }
            _teamItems.Clear();
            _teams.Clear();
        }
    }
}