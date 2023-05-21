using Cysharp.Threading.Tasks;

namespace Services.Common
{
    public abstract class Service : IService
    {
        public ServiceState State { get; private set; } = ServiceState.Created;

        public async UniTask Initialize()
        {
            await OnInitialize();
            State = ServiceState.Initialized;
        }

        public async UniTask Start()
        {
            await OnStart();
            State = ServiceState.Started;
        }

        public async UniTask Dispose()
        {
            await OnDispose();
            State = ServiceState.Disposed;
        }

        protected virtual UniTask OnInitialize() => UniTask.CompletedTask;
        protected virtual UniTask OnStart() => UniTask.CompletedTask;
        protected virtual UniTask OnDispose() => UniTask.CompletedTask;
    }
}