using Cysharp.Threading.Tasks;

namespace Common.StateMachine
{
    public abstract class GameState
    {
        internal async UniTask EnterState()
        {
            await OnEnterState();
        }

        internal async UniTask StartState()
        {
            await OnStartState();
        }

        internal async UniTask StopState()
        {
            await OnStopState();
        }

        internal async UniTask ExitState()
        {
            await OnExitState();
        }

        /// <summary>
        /// After Current State Stop
        /// </summary>
        protected virtual UniTask OnEnterState() => UniTask.CompletedTask;

        /// <summary>
        /// After Current State Exit
        /// </summary>
        protected virtual UniTask OnStartState() => UniTask.CompletedTask;

        /// <summary>
        /// Before New State State Enter
        /// </summary>
        protected virtual UniTask OnStopState() => UniTask.CompletedTask;

        /// <summary>
        /// Before New State State Start
        /// </summary>
        protected virtual UniTask OnExitState() => UniTask.CompletedTask;
    }
}