using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Services.UI;

namespace Game.UI.Popups.Teams
{
    public record TeamsPopupModel(List<Team> Teams) : UIModel
    {
        public List<Team> Teams { get; } = Teams;

        private UniTaskCompletionSource<Team> _completionSource;

        public UniTask<Team> WaitForTeam()
        {
            _completionSource = new UniTaskCompletionSource<Team>();
            return _completionSource.Task;
        }

        public void EndChoose(Team team)
        {
            _completionSource?.TrySetResult(team);
        }
    }
}