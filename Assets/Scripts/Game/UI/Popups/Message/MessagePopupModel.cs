using Services.UI;

namespace Game.UI.Popups.Message
{
    public record MessagePopupModel(string Message) : UIModel
    {
        public string Message { get; } = Message;
    }
}