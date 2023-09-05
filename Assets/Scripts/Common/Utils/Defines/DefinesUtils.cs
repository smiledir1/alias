#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

namespace Common.Utils.Defines
{
    public static class DefinesUtils
    {
        private static readonly List<NamedBuildTarget> DefaultBuildTargets = new()
        {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android,
            NamedBuildTarget.iOS,
            NamedBuildTarget.WebGL
        };

        public static void AddDefinesForDefaultTargets(List<string> defines)
        {
            PlayerSettings.GetScriptingDefineSymbols(
                NamedBuildTarget.Standalone,
                out var projectDefines);
            var addDefines = new List<string>(projectDefines);

            foreach (var define in defines)
            {
                if (string.IsNullOrEmpty(define)) continue;
                if (addDefines.Contains(define)) continue;
                addDefines.Add(define);
            }

            SetDefinesForDefaultTargets(addDefines);
        }

        public static void RemoveDefinesForDefaultTargets(List<string> defines)
        {
            PlayerSettings.GetScriptingDefineSymbols(
                NamedBuildTarget.Standalone,
                out var projectDefines);
            var addDefines = new List<string>(projectDefines);

            foreach (var define in defines)
            {
                if (string.IsNullOrEmpty(define)) continue;
                if (!addDefines.Contains(define)) continue;
                addDefines.Remove(define);
            }

            SetDefinesForDefaultTargets(addDefines);
        }

        public static void SetDefinesForDefaultTargets(List<string> defines)
        {
            if (defines.Count == 0) return;

            var definesArray = defines.ToArray();
            foreach (var buildTarget in DefaultBuildTargets)
            {
                PlayerSettings.SetScriptingDefineSymbols(buildTarget, definesArray);
            }
        }

        public static void AddDefinesTarget(List<string> defines, NamedBuildTarget target)
        {
            PlayerSettings.GetScriptingDefineSymbols(
                target,
                out var projectDefines);
            var addDefines = new List<string>(projectDefines);

            foreach (var define in defines)
            {
                if (string.IsNullOrEmpty(define)) continue;
                if (addDefines.Contains(define)) continue;
                addDefines.Add(define);
            }

            SetDefinesTargets(addDefines, new List<NamedBuildTarget> {target});
        }

        public static void RemoveDefinesTarget(List<string> defines, NamedBuildTarget target)
        {
            PlayerSettings.GetScriptingDefineSymbols(
                target,
                out var projectDefines);
            var addDefines = new List<string>(projectDefines);

            foreach (var define in defines)
            {
                if (string.IsNullOrEmpty(define)) continue;
                if (!addDefines.Contains(define)) continue;
                addDefines.Remove(define);
            }

            SetDefinesTargets(addDefines, new List<NamedBuildTarget> {target});
        }

        public static void SetDefinesTargets(List<string> defines, List<NamedBuildTarget> targets)
        {
            if (defines.Count == 0) return;

            var definesArray = defines.ToArray();
            foreach (var buildTarget in targets)
            {
                if (buildTarget == NamedBuildTarget.Unknown) continue;
                PlayerSettings.SetScriptingDefineSymbols(buildTarget, definesArray);
            }
        }
    }
}
#endif