using Cysharp.Threading.Tasks;

namespace Services.UI.ScreenService
{
    public interface IScreenService : IUIService
    {
        public UniTask<T> ShowAsync<T>(
            UIModel uiModel,
            int group = default) where T : UIObject;
    }
}