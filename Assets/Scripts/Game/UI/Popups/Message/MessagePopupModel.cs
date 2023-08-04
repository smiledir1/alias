using Services.UI;

namespace Game.UI.Popups.Message
{
    public record MessagePopupModel(
        string Title,
        string Message,
        bool Localize = false) : UIModel
    {
        public string Title { get; } = Title;
        public string Message { get; } = Message;
        public bool Localize { get; } = Localize;
    }
}