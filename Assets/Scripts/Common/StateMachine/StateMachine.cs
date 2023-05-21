using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.StateMachine
{
    public static class StateMachine
    {
        private static GameState _currentGameState;
        private static bool _isChangingState;

        public static async UniTask GoToState(this GameState newGameState)
        {
            if (_isChangingState)
            {
                Debug.LogError("An attempt to change the state while the state change process is running");
                return;
            }

            _isChangingState = true;
            
            if (_currentGameState != null) await _currentGameState.StopState();
            await newGameState.EnterState();

            if (_currentGameState != null) await _currentGameState.ExitState();
            await newGameState.StartState();

            _currentGameState = newGameState;

            _isChangingState = false;
        }
    }
}