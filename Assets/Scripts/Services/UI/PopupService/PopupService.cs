using Cysharp.Threading.Tasks;
using Services.Assets;

namespace Services.UI.PopupService
{
    public class PopupService : UIService, IPopupService
    {
        protected override UICanvas Canvas => _popupCanvas;

        private PopupCanvas _popupCanvas;

        public PopupService(IAssetsService assetsService) : base(assetsService)
        {
        }

        protected override async UniTask OnInitialize()
        {
            await base.OnInitialize();
            _popupCanvas = await AssetsService.InstantiateAsync<PopupCanvas>();
            _popupCanvas.DisableRaycast();
        }
    }
}