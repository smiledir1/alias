using Cysharp.Threading.Tasks;
using Services.UI;

namespace Game.UI.Screens.Game
{
    public record RoundScreenModel : UIModel
    {
        private UniTaskCompletionSource<bool> _completionSource;
        
        public UniTask<bool> WaitForRound()
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            return _completionSource.Task;
        }
        
        public void EndRound()
        {
            _completionSource?.TrySetResult(true);
        }

        public void GoBack()
        {
            _completionSource?.TrySetResult(false);
        }
    }
}