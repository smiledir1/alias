using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine.Events;

namespace Services.YandexGames
{
    public interface IYandexGamesService : IService
    {
        UniTask<PlayerData> InitializePlayer(PlayerPhotoSize photoSize);
        UniTask ShowFullscreenAd();
        UniTask ShowRewardedVideoAd(UnityAction onRewardedCallback = null);
        UniTask<EnvironmentData> GetEnvironment();
        UniTask<ReviewReason> ShowReview();
    }
}