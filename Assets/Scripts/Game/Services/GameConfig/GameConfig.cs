using UnityEngine;

namespace Game.Services.GameConfig
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private bool _isDebug;

        [SerializeField]
        private int _frameRate;

        [SerializeField]
        private PlatformType _platformType;

        public bool IsDebug => _isDebug;
        public int FrameRate => _frameRate;
        public PlatformType PlatformType => _platformType;
    }

    public enum PlatformType
    {
        None,
        Yandex,
        GooglePlay,
        AppStore
    }
}