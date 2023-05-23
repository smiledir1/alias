using Services.Common;

namespace Game.Services.WordsPacks
{
    public interface IWordsPacksService : IService
    {
        WordsPacksConfig WordsPacksConfig { get; }
    }
}