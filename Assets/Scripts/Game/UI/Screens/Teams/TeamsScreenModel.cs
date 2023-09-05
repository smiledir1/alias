using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Services.UI;

namespace Game.UI.Screens.Teams
{
    public record TeamsScreenModel(
        WordsPacksConfigItem WordsPacksConfigItem,
        int RoundTimeSeconds,
        bool IsUnlimitedTimeForLastWord,
        List<Team> Teams,
        int CurrentRound) : UIModel
    {
        public WordsPacksConfigItem WordsPacksConfigItem { get; } = WordsPacksConfigItem;
        public int RoundTimeSeconds { get; } = RoundTimeSeconds;
        public bool IsUnlimitedTimeForLastWord { get; } = IsUnlimitedTimeForLastWord;
        public List<Team> Teams { get; } = Teams;
        public int CurrentRound { get; } = CurrentRound;

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