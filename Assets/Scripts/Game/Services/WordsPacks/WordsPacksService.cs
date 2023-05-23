using Cysharp.Threading.Tasks;
using Services.Assets;
using Services.Common;

namespace Game.Services.WordsPacks
{
    public class WordsPacksService : Service, IWordsPacksService
    {
        #region Services

        private readonly IAssetsService _assetsService;

        #endregion

        public WordsPacksConfig WordsPacksConfig { get; private set; }

        public WordsPacksService(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        protected override async UniTask OnInitialize()
        {
            WordsPacksConfig = await _assetsService.LoadAsset<WordsPacksConfig>();
        }
    }
}