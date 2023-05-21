using UnityEngine;

namespace Game.Services.GameConfig
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private bool _isDebug;

        public bool IsDebug => _isDebug;
        
        private void OnValidate()
        {
            //TODO: if debug or release change ENV_DEV RELEASE_ENV
            //TODO: make Debug Types
        }
    }
}