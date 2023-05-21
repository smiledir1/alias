using Cysharp.Threading.Tasks;
using Services.Common;

namespace Services.Audio
{
    public interface IAudioService : IService
    {
        float SfxVolume { get; set; }
        float MusicVolume { get; set; }
        UniTask PlayMusic(string id);
        UniTask StopMusic(string id);
        UniTask PlaySound(string id, bool multiSound = false);
    }
}