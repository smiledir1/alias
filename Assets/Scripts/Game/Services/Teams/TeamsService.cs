using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.UserData.Game;
using Services.Assets;
using Services.Common;
using Services.Localization;

namespace Game.Services.Teams
{
    public class TeamsService : Service, ITeamsService
    {
        #region Services

        private readonly ILocalizationService _localizationService;
        private readonly IAssetsService _assetsService;

        #endregion

        public TeamsConfig TeamsConfig { get; private set; }

        public TeamsService(
            IAssetsService assetsService,
            ILocalizationService localizationService)
        {
            _assetsService = assetsService;
            _localizationService = localizationService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);
            await WaitForServiceInitialize(_localizationService);

            TeamsConfig = await _assetsService.LoadAsset<TeamsConfig>();
        }

        public Team CreateTeamFromConfig(TeamItem item)
        {
            var teamName = _localizationService.GetText(item.NameLocalizationKey);
            return new Team(teamName, item.Id, item.Icon, 0);
        }

        public Team CreateTeamFromData(TeamData data)
        {
            var item = TeamsConfig.teams.Find(t => t.Id == data.id);
            var teamName = _localizationService.GetText(item.NameLocalizationKey);
            return new Team(teamName, item.Id, item.Icon, data.score);
        }

        public List<Team> CreateTeamsFromData(List<TeamData> data)
        {
            var teamList = new List<Team>();
            foreach (var d in data)
            {
                var team = CreateTeamFromData(d);
                teamList.Add(team);
            }

            return teamList;
        }

        public TeamData CreateDataFromTeam(Team team) => new(team.Id, team.Score);

        public List<TeamData> CreateDataFromTeams(List<Team> teams)
        {
            var teamList = new List<TeamData>();
            foreach (var d in teams)
            {
                var team = CreateDataFromTeam(d);
                teamList.Add(team);
            }

            return teamList;
        }
    }
}