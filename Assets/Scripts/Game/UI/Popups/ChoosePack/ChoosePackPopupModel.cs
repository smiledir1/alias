using Cysharp.Threading.Tasks;
using Game.Services.WordsPacks;
using Services.UI;

namespace Game.UI.Popups.ChoosePack
{
    public record ChoosePackPopupModel : UIModel
    {
        private UniTaskCompletionSource<WordsPacksConfigItem> _completionSource;

        public UniTask<WordsPacksConfigItem> WaitForPackItem()
        {
            _completionSource = new UniTaskCompletionSource<WordsPacksConfigItem>();
            return _completionSource.Task;
        }

        public void SetChoosePack(WordsPacksConfigItem wordsPacksConfigItem)
        {
            _completionSource?.TrySetResult(wordsPacksConfigItem);
        }
    }
}