using Cysharp.Threading.Tasks;

namespace Services.UI
{
    public abstract record UIModel
    {
        private UniTaskCompletionSource _closeCompletionSource;

        public UniTask WaitForClose()
        {
            _closeCompletionSource = new UniTaskCompletionSource();
            return _closeCompletionSource.Task;
        }

        internal void Close()
        {
            _closeCompletionSource?.TrySetResult();
        }
    }
}