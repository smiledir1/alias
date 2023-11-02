using Cysharp.Threading.Tasks;

namespace Services.UI
{
    public abstract record UIModel
    {
        public bool IsOpened { get; private set; }
        private UniTaskCompletionSource _closeCompletionSource;

        public UniTask WaitForClose()
        {
            _closeCompletionSource = new UniTaskCompletionSource();
            return _closeCompletionSource.Task;
        }

        internal void Open()
        {
            IsOpened = true;
        }

        internal void Close()
        {
            _closeCompletionSource?.TrySetResult();
            IsOpened = false;
        }
    }
}