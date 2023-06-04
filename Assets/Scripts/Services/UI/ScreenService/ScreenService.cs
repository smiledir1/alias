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
            _screenCanvas = await AssetsService.InstantiateAsync<ScreenCanvas>();
            await base.OnInitialize();
        }

        public async UniTask<T> ShowAsync<T>(
            UIModel uiModel,
            int group = default) where T : UIObject
        {
            return await base.ShowAsync<T>(uiModel, true, group);
        }
    }
}