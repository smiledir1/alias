using System.Collections.Generic;
using Game.UserData.Game;
using Services.Common;

namespace Game.Services.Teams
{
    public interface ITeamsService : IService
    {
        TeamsConfig TeamsConfig { get; }
        Team CreateTeamFromConfig(TeamItem item);
        Team CreateTeamFromData(TeamData data);
        List<Team> CreateTeamsFromData(List<TeamData> datas);
        TeamData CreateDataFromTeam(Team team);
        List<TeamData> CreateDataFromTeams(List<Team> teams);
    }
}