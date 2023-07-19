using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;

namespace Game.Services.GameConfig
{
    public class GameConfigService : Service, IGameConfigService
    {
        #region Services

        private readonly IAssetsService _assetsService;

        #endregion

        public GameConfig GameConfig { get; private set; }

        public GameConfigService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            await WaitForServiceInitialize(_assetsService);
            
            GameConfig = await _assetsService.LoadAsset<GameConfig>();
        }
    }
}