#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Services.Audio.Components.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioComponent), true)]
    public class AudioComponentEditor : UnityEditor.Editor
    {
        private const string PathToConfigs = "Assets/Configs";
        private const int DrawCount = 10;
        
        private AudioComponent _audioComponent;
        private AudioConfig _audioConfig;
        
        private readonly HashSet<string> _entryKeys = new();
        private readonly List<string> _findKeys = new();
        private bool _notFoundKeys;

        private void OnEnable()
        {
            _audioComponent = target as AudioComponent;
            
            var assetsFilter = $"t:{nameof(AudioConfig)}";
            var searchFolder = new[] {PathToConfigs};
            var assets = AssetDatabase.FindAssets(assetsFilter, searchFolder);
            if (assets.Length == 0)
            {
                Debug.LogError($"Create {nameof(AudioConfig)} First");
                return;
            }
            
            var configGuid = assets[0];
            var configPath = AssetDatabase.GUIDToAssetPath(configGuid);
            _audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>(configPath);

            foreach (var sound in _audioConfig.Sounds)
            {
                _entryKeys.Add(sound.Id);
            }
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawAudioHelper();
        }

        private void DrawAudioHelper()
        {
            GUILayout.Space(10);
            GUILayout.Label("Helper");
            
            var componentKey = _audioComponent.SoundKey;
            if (string.IsNullOrEmpty(componentKey)) return;
            
            if (GUI.changed)
            {
                _findKeys.Clear();
                EditorUtility.SetDirty(_audioComponent);
            }
            
            if (!_entryKeys.Contains(componentKey))
            {
                DrawContainsKey(componentKey);
                return;
            }

            DrawUnContainsKey(componentKey);
        }

        private void DrawContainsKey(string componentKey)
        {
            GUILayout.Label("Нажми, чтобы выбрать", EditorStyles.boldLabel);

            if (_findKeys.Count == 0)
            {
                var drawCount = 0;
                foreach (var entryKey in _entryKeys)
                {
                    var hasParameter = entryKey.Contains(componentKey, StringComparison.Ordinal);
                   
                    if (hasParameter)
                    {
                        _findKeys.Add(entryKey);
                        if (GUILayout.Button(entryKey, "CN CountBadge"))
                        {
                            GUI.FocusControl(null);
                            _audioComponent.SoundKey = entryKey;
                            EditorUtility.SetDirty(_audioComponent);
                        }

                        drawCount++;
                    }
                    
                    if (drawCount >= DrawCount)
                    {
                        break;
                    }
                }
                _notFoundKeys = drawCount == 0;
            }
            else
            {
                foreach (var findKey in _findKeys)
                {
                    if (GUILayout.Button(findKey, "CN CountBadge"))
                    {
                        GUI.FocusControl(null);
                        _audioComponent.SoundKey = findKey;
                        EditorUtility.SetDirty(_audioComponent);
                    }
                }
            }
            
            if (_findKeys.Count >= DrawCount)
            {
                GUILayout.Label("Еще..");
            }

            if (_notFoundKeys)
            {
                GUILayout.Box("Не найдено");
            }
        }

        private void DrawUnContainsKey(string componentKey)
        {
            GUILayout.Label("Ключ есть в конфиге ключей", EditorStyles.boldLabel);
        }
    }
}
#endif