#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Services.GameConfig.Editor
{
    public static class GameConfigEditor
    {
        private const string _pathToConfigs = "Assets/Configs";

        [MenuItem("Tools/GameConfig/Open GameConfig")]
        private static void OpenGameConfigConfig()
        {
            var assetsFilter = $"t:{nameof(GameConfig)}";
            var searchFolder = new[] {_pathToConfigs};
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
}
#endif