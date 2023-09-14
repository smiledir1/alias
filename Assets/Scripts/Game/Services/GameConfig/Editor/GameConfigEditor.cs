#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Game.Services.GameConfig.Editor
{
    [CustomEditor(typeof(GameConfig), true)]
    public class GameConfigEditor : UnityEditor.Editor
    {
        private const string DevDefine = "DEV_ENV";
        private const string PathToConfigs = "Assets/Configs";

        private GameConfig _gameConfig;
        private bool _prevIsDebug;
        private PlatformType _prevPlatformType;

        protected void OnEnable()
        {
            _gameConfig = target as GameConfig;
            if (_gameConfig != null)
            {
                _prevPlatformType = _gameConfig.PlatformType;
                _prevIsDebug = _gameConfig.IsDebug;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var selectedBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var selectedBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(selectedBuildTarget);
            var selectedBuildTargetName = NamedBuildTarget.FromBuildTargetGroup(selectedBuildTargetGroup);
            var platformBuildTargetName = PlatformTypeConverter.GetBuildTarget(_gameConfig.PlatformType);

            if (platformBuildTargetName != selectedBuildTargetName)
            {
                var helpBoxText = $"Change Platform to {platformBuildTargetName.TargetName}";
                EditorGUILayout.HelpBox(helpBoxText, MessageType.Warning);
            }

            if (_prevIsDebug != _gameConfig.IsDebug)
            {
                if (_gameConfig.IsDebug)
                    Common.Utils.Defines.DefinesUtils.AddDefinesForDefaultTargets(new List<string> {DevDefine});
                else
                    Common.Utils.Defines.DefinesUtils.RemoveDefinesForDefaultTargets(new List<string> {DevDefine});

                _prevIsDebug = _gameConfig.IsDebug;
            }

            if (_prevPlatformType != _gameConfig.PlatformType)
            {
                var newDefines = new List<string> {PlatformTypeConverter.GetDefineName(_gameConfig.PlatformType)};
                var newDefinesTargets = PlatformTypeConverter.GetBuildTarget(_gameConfig.PlatformType);
                var prevDefines = new List<string> {PlatformTypeConverter.GetDefineName(_prevPlatformType)};
                var prevDefinesTargets = PlatformTypeConverter.GetBuildTarget(_prevPlatformType);
                Common.Utils.Defines.DefinesUtils.RemoveDefinesTarget(prevDefines, prevDefinesTargets);
                Common.Utils.Defines.DefinesUtils.AddDefinesTarget(newDefines, newDefinesTargets);
                _prevPlatformType = _gameConfig.PlatformType;
            }
        }

        [MenuItem("Tools/GameConfig/Open GameConfig")]
        private static void OpenGameConfigConfig()
        {
            var assetsFilter = $"t:{nameof(GameConfig)}";
            var searchFolder = new[] {PathToConfigs};
            var assets = AssetDatabase.FindAssets(assetsFilter, searchFolder);
            if (assets.Length == 0)
            {
                Debug.LogError($"Create {nameof(GameConfig)} First");
                return;
            }

            var configGuid = assets[0];
            var configPath = AssetDatabase.GUIDToAssetPath(configGuid);
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>(configPath);
            if (config == null) return;

            Selection.activeObject = config;
        }
    }

    public static class PlatformTypeConverter
    {
        private static readonly Dictionary<PlatformType, string> PlatformTypeToDefine = new()
        {
            {PlatformType.None, string.Empty},
            {PlatformType.Yandex, "YANDEX_PLATFORM"},
            {PlatformType.GooglePlay, "GOOGLE_PLAY_PLATFORM"},
            {PlatformType.AppStore, "APP_STORE_PLATFORM"},
            {PlatformType.RuStore, "RU_STORE_PLATFORM"}
        };

        public static string GetDefineName(PlatformType platformType) =>
            PlatformTypeToDefine[platformType];

        public static NamedBuildTarget GetBuildTarget(PlatformType platformType)
        {
            return platformType switch
            {
                PlatformType.None => NamedBuildTarget.Unknown,
                PlatformType.Yandex => NamedBuildTarget.WebGL,
                PlatformType.GooglePlay => NamedBuildTarget.Android,
                PlatformType.AppStore => NamedBuildTarget.iOS,
                PlatformType.RuStore => NamedBuildTarget.Android,
                _ => throw new ArgumentOutOfRangeException(nameof(platformType), platformType, null)
            };
        }

        public static List<string> GetAllDefinesNames() =>
            new(PlatformTypeToDefine.Values);
    }
}
#endif