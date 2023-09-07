using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Services.Helper;
using Services.UI.PopupService;
using UnityEngine;

namespace Game.UI.Popups.Teams
{
    public class TeamsPopup : Popup<TeamsPopupModel>
    {
        [SerializeField]
        private TeamPopupItem _templatePopupItem;

        [Service]
        private static ITeamsService _teamsService;

        private readonly List<TeamPopupItem> _teams = new();

        protected override UniTask OnOpenAsync()
        {
            _templatePopupItem.gameObject.SetActive(false);
            foreach (var teamPopupItem in _teams)
            {
                Destroy(teamPopupItem.gameObject);
            }

            _teams.Clear();

            var parent = _templatePopupItem.transform.parent;
            var teams = _teamsService.TeamsConfig.teams;
            foreach (var teamItem in teams)
            {
                var team = _teamsService.CreateTeamFromConfig(teamItem);
                if (Model.Teams.Exists(t => t.Name == team.Name)) continue;
                var teamPopupItem = Instantiate(_templatePopupItem, parent);
                teamPopupItem.Initialize(team, OnTeamPopupItemClick);
                _teams.Add(teamPopupItem);
                teamPopupItem.gameObject.SetActive(true);
            }

            return base.OnOpenAsync();
        }

        private void OnTeamPopupItemClick(Team team)
        {
            Model.EndChoose(team);
            Close();
        }
    }
}