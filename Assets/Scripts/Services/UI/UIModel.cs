using Cysharp.Threading.Tasks;

namespace Services.UI
{
    public abstract record UIModel
    {
        private UniTaskCompletionSource _completionSource;

        public UniTask WaitForClose()
        {
            _completionSource = new UniTaskCompletionSource();
            return _completionSource.Task;
        }

        internal void Close()
        {
            _completionSource?.TrySetResult();
        }
    }
}