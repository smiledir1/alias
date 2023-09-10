using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Common.CheckProject.Editor.NullReferenceDetection.Editor
{
    public static class ExecuteFindNullReferences
    {
        [MenuItem("Tools/CheckCode/Check Current Prefab (not UNITY & TMP)")]
        public static void CheckCurrentPrefab()
        {
            var curPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (curPrefabStage == null)
            {
                Debug.LogError("Null prefab");
                return;
            }

            var prefabRoot = curPrefabStage.prefabContentsRoot;
            var detector = new NullReferenceDetector();
            var nullReferences = detector.FindAllNullReferences(IsNotUnity,
                ExtensionMethods.LoadIgnoreList(),
                new List<GameObject> {prefabRoot}).ToList();

            ShowFindNullReferences(nullReferences);
            Debug.Log("Find Complete");
        }

        // [MenuItem("Tools/Find Null References")]
        public static void Execute()
        {
            CheckForNullReferences(IsVisible);
        }

        //[MenuItem("Tools/Find Null References 2")]
        public static void Execute2()
        {
            var curPrefab = PrefabStageUtility.GetCurrentPrefabStage()
                .prefabContentsRoot;
            Debug.Log(curPrefab.name);
            var detector = new NullReferenceDetector();
            var nullReferences = detector.FindAllNullReferences(IsNotUnity,
                ExtensionMethods.LoadIgnoreList(),
                new List<GameObject> {curPrefab}).ToList();

            ShowFindNullReferences(nullReferences);
        }

        public static bool CheckForNullReferences(Func<NullReference, bool> filter)
        {
            var detector = new NullReferenceDetector();
            var nullReferences = detector.FindAllNullReferences(filter,
                ExtensionMethods.LoadIgnoreList(),
                ExtensionMethods.LoadPrefabList()).ToList();

            foreach (var nullReference in nullReferences)
            {
                var fieldName = ObjectNames.NicifyVariableName(nullReference.FieldName);
                var color = ColorFor(nullReference);

                var message = string.Format(
                    "Null reference found in <b>{0}</b> > <b>{1}</b> > <color={2}><b>{3}</b></color>\n",
                    nullReference.GameObjectName,
                    nullReference.ComponentName,
                    color,
                    fieldName);

                Debug.Log(message, nullReference.GameObject);
            }

            return nullReferences.Any();
        }

        public static bool IsNotUnity(NullReference nullReference) =>
            !nullReference.ComponentName.Contains("UnityEngine") &&
            !nullReference.ComponentName.Contains("TMPro");

        public static bool IsVisible(NullReference nullReference) =>
            PreferencesStorage.IsVisible(nullReference.AttributeIdentifier);

        public static string ColorFor(NullReference nullReference)
        {
            var color = PreferencesStorage.ColorFor(nullReference.AttributeIdentifier);
            return string.Format("#{0}", ColorUtility.ToHtmlStringRGB(color));
        }

        private static void ShowFindNullReferences(List<NullReference> nullReferences)
        {
            foreach (var nullReference in nullReferences)
            {
                var fieldName = ObjectNames.NicifyVariableName(nullReference.FieldName);
                var color = ColorFor(nullReference);

                var message = string.Format(
                    "Null reference found in <b>{0}</b> > <b>{1}</b> > <color={2}><b>{3}</b></color>\n",
                    nullReference.GameObjectName,
                    nullReference.ComponentName,
                    color,
                    fieldName);

                Debug.Log(message, nullReference.GameObject);
            }
        }
    }
}