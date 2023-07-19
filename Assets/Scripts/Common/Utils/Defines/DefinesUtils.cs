#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Common.Utils.Defines
{
    public static class DefinesUtils
    {
        private static readonly List<UnityEditor.Build.NamedBuildTarget> BuildTargets = new()
        {
            UnityEditor.Build.NamedBuildTarget.Standalone,
            UnityEditor.Build.NamedBuildTarget.Android,
            UnityEditor.Build.NamedBuildTarget.iOS,
            UnityEditor.Build.NamedBuildTarget.WebGL
        };
        
        public static void SetDefinesForTargets(List<string> defines)
        {
            if (defines.Count == 0) return;

            var definesArray = defines.ToArray();
            foreach (var buildTarget in BuildTargets)
            {
                PlayerSettings.SetScriptingDefineSymbols(buildTarget, definesArray);
            }
        }
    }
}
#endif