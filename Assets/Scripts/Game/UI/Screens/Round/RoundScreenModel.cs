using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Services.UI;

namespace Game.UI.Screens.Round
{
    public record RoundScreenModel(
        Team RoundTeam,
        int RoundTimeSeconds,
        WordsPacksConfigItem WordsPacksConfigItem,
        bool IsUnlimitedTimeForLastWord) : UIModel
    {
        public Team RoundTeam { get; } = RoundTeam;
        public int RoundTimeSeconds { get; } = RoundTimeSeconds;
        public WordsPacksConfigItem WordsPacksConfigItem { get; } = WordsPacksConfigItem;
        public bool IsUnlimitedTimeForLastWord { get; } = IsUnlimitedTimeForLastWord;

        private UniTaskCompletionSource<List<RoundWord>> _completionSource;

        public UniTask<List<RoundWord>> WaitForRound()
        {
            _completionSource = new UniTaskCompletionSource<List<RoundWord>>();
            return _completionSource.Task;
        }
        
        public void EndRound(List<RoundWord> words)
        {
            _completionSource?.TrySetResult(words);
        }

        public void GoBack()
        {
            _completionSource?.TrySetResult(null);
        }
    }
}