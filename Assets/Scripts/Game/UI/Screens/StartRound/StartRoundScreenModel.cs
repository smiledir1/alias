using Cysharp.Threading.Tasks;
using Services.UI;

namespace Game.UI.Screens.StartGame
{
    public record StartRoundScreenModel : UIModel
    {
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