using System;
using Services.UI;

namespace Game.UI.Popups.Confirm
{
    public record ConfirmPopupModel(
        string Message,
        Action ConfirmCallback = null) : UIModel
    {
        public string Message { get; } = Message;
        public Action ConfirmCallback { get; } = ConfirmCallback;
    }
}