using System;
using Cysharp.Threading.Tasks;
using Services.UI;

namespace Game.UI.Popups.Confirm
{
    public record ConfirmPopupModel(
        string Title,
        string Message,
        Action ConfirmCallback = null,
        bool Localize = false) : UIModel
    {
        public string Title { get; } = Title;
        public string Message { get; } = Message;
        public Action ConfirmCallback { get; } = ConfirmCallback;
        public bool Localize { get; } = Localize;

        private UniTaskCompletionSource<bool> _confirmSource;

        public UniTask WaitForConfirm()
        {
            _confirmSource = new UniTaskCompletionSource<bool>();
            return _confirmSource.Task;
        }

        public void IsConfirm(bool confirm)
        {
            _confirmSource?.TrySetResult(confirm);
        }
    }
}