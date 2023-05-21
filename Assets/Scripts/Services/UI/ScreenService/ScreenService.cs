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
    }
}