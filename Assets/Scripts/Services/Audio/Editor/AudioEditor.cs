#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Audio.Editor
{
    public static class AudioEditor
    {
        private const string _pathToConfigs = "Assets/Configs";
        private static readonly string AssetsFilter = $"t:{nameof(AudioConfig)}";
        private static readonly string[] SearchFolder = {_pathToConfigs};
        
        [MenuItem("Tools/Audio/Open AudioConfig")]
        private static void OpenAudioConfig()
        {
            var assets = AssetDatabase.FindAssets(AssetsFilter, SearchFolder);
            if (assets.Length == 0)
            {
                Debug.LogError($"Create {nameof(AudioConfig)} First");
                return;
            }

            var configGuid = assets[0];
            var configPath = AssetDatabase.GUIDToAssetPath(configGuid);
            var config = AssetDatabase.LoadAssetAtPath<AudioConfig>(configPath);
            if (config == null) return;

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
        
        private static void AddAssetToSounds(SoundType type)
        {
            var assets = AssetDatabase.FindAssets(AssetsFilter, SearchFolder);
            if (assets.Length == 0) return;
            
            var configGuid = assets[0];
            var configPath = AssetDatabase.GUIDToAssetPath(configGuid);
            var config = AssetDatabase.LoadAssetAtPath<AudioConfig>(configPath);
            if (config == null) return;
            
            foreach (var guid in Selection.assetGUIDs)
            {
                var referenceClip = new AssetReferenceT<AudioClip>(guid);
                var assetName = referenceClip.editorAsset.name;
                var setting = new SoundSettings(referenceClip, type, assetName);
                config.AddSoundSettings(setting);
            }
        }
    }
}
#endif