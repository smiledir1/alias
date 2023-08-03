using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Common.Utils.Defines;
#endif

namespace Game.Services.GameConfig
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private bool _isDebug;

        [SerializeField]
        private int _frameRate;
        
        public bool IsDebug => _isDebug;
        public int FrameRate => _frameRate;
        
        
#if UNITY_EDITOR

        private const string DevDefine = "DEV_ENV";

        private void OnValidate()
        {
            UnityEditor.PlayerSettings.GetScriptingDefineSymbols(
                UnityEditor.Build.NamedBuildTarget.Standalone,
                out var projectDefines);
            var addDefines = new List<string>(projectDefines);
            
            if (_isDebug)
            {
                addDefines.Add(DevDefine);
            }
            else
            {
                addDefines.Remove(DevDefine);
            }

            DefinesUtils.SetDefinesForTargets(addDefines);
        }
        
#endif
    }
}