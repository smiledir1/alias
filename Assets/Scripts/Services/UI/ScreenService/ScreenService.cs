using Cysharp.Threading.Tasks;
using Services.Assets;

namespace Services.UI.ScreenService
{
    public class ScreenService : UIService, IScreenService
    {
        protected override UICanvas Canvas => _screenCanvas;

        private ScreenCanvas _screenCanvas;

        public ScreenService(IAssetsService assetsService) : base(assetsService)
        {
        }

        protected override async UniTask OnInitialize()
        {
            await base.OnInitialize();
            _screenCanvas = await AssetsService.InstantiateAsync<ScreenCanvas>();
        }

        public async UniTask<T> ShowAsync<T>(
            UIModel uiModel,
            int group = default) where T : UIObject =>
            await base.ShowAsync<T>(uiModel, true, group);
    }
}