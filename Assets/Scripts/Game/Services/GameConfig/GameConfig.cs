using UnityEngine;

namespace Game.Services.GameConfig
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private bool isDebug;

        [SerializeField]
        private int frameRate;

        [SerializeField]
        private PlatformType platformType;

        public bool IsDebug => isDebug;
        public int FrameRate => frameRate;
        public PlatformType PlatformType => platformType;
    }

    public enum PlatformType
    {
        None,
        Yandex,
        GooglePlay,
        AppStore,
        RuStore
    }
}