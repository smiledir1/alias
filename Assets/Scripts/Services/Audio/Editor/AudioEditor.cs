#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Audio.Editor
{
    public static class AudioEditor
    {
        private const string PathToConfigs = "Assets/Configs";
        private static readonly string AssetsFilter = $"t:{nameof(AudioConfig)}";
        private static readonly string[] SearchFolder = {PathToConfigs};

        [MenuItem("Tools/Audio/Open AudioConfig")]
        private static void OpenAudioConfig()
        {
            var config = GetAudioConfig();
            Selection.activeObject = config;
        }

        [MenuItem("Assets/Audio/Add Music", true)]
        [MenuItem("Assets/Audio/Add SFX", true)]
        private static bool AddAssetToSoundsValidate()
        {
            foreach (var selection in Selection.objects)
            {
                if (!(selection is AudioClip)) return false;
            }

            return true;
        }

        [MenuItem("Assets/Audio/Add SFX")]
        private static void AddAssetToSoundsSfx()
        {
            AddAssetToSounds(SoundType.Sfx);
        }

        [MenuItem("Assets/Audio/Add Music")]
        private static void AddAssetToSoundsMusic()
        {
            AddAssetToSounds(SoundType.Music);
        }

        [MenuItem("Tools/Audio/Create Audio Constants")]
        private static void CreateAudioConfigConstants()
        {
            var pathToScriptsDirectory = Path.Combine(Application.dataPath, "Scripts");
            var pathToGameDirectory = Path.Combine(pathToScriptsDirectory, "Game");
            var pathToConstantsDirectory = Path.Combine(pathToGameDirectory, "Constants");
            var pathToFileConstants = Path.Combine(pathToConstantsDirectory, "AudioConstants.cs");

            var fileText = "public static class AudioConstants {";

            var config = GetAudioConfig();

            foreach (var sound in config.Sounds)
            {
                fileText += $"public const string {sound.Id}_a = \"{sound.Id}\"; ";
            }

            fileText += "}";

            Directory.CreateDirectory(pathToConstantsDirectory);
            File.WriteAllText(pathToFileConstants, fileText);
            AssetDatabase.Refresh();
            Debug.Log("Constants Created");
        }

        private static void AddAssetToSounds(SoundType type)
        {
            var config = GetAudioConfig();

            foreach (var guid in Selection.assetGUIDs)
            {
                var referenceClip = new AssetReferenceT<AudioClip>(guid);
                var assetName = referenceClip.editorAsset.name;
                var setting = new SoundSettings(referenceClip, type, assetName);
                config.AddSoundSettings(setting);
            }
        }

        private static AudioConfig GetAudioConfig()
        {
            var assets = AssetDatabase.FindAssets(AssetsFilter, SearchFolder);
            if (assets.Length == 0)
            {
                Debug.LogError($"Create {nameof(AudioConfig)} First");
                return null;
            }

            var configGuid = assets[0];
            var configPath = AssetDatabase.GUIDToAssetPath(configGuid);
            var config = AssetDatabase.LoadAssetAtPath<AudioConfig>(configPath);
            return config;
        }
    }
}
#endif