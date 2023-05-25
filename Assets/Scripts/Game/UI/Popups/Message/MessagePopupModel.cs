using Services.UI;

namespace Game.UI.Popups.Message
{
    public record MessagePopupModel(
        string Title,
        string Message) : UIModel
    {
        public string Title { get; } = Title;
        public string Message { get; } = Message;
    }
}