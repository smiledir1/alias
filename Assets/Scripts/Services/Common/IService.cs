using Cysharp.Threading.Tasks;

namespace Services.Common
{
    public interface IService
    {
        ServiceState State { get; }
        UniTask Initialize();
        UniTask Dispose();
    }
}