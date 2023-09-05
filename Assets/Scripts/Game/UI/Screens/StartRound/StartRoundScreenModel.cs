using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Services.UI;

namespace Game.UI.Screens.StartRound
{
    public record StartRoundScreenModel(Team RoundTeam) : UIModel
    {
        public Team RoundTeam { get; } = RoundTeam;

        private UniTaskCompletionSource<bool> _completionSource;


        public UniTask<bool> WaitForStart()
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            return _completionSource.Task;
        }

        public void StartRound()
        {
            _completionSource?.TrySetResult(true);
        }

        public void GoBack()
        {
            _completionSource?.TrySetResult(false);
        }
    }
}