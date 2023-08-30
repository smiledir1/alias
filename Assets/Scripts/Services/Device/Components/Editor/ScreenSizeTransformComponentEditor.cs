#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Services.Device.Components.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScreenSizeTransformComponent), true)]
    public class ScreenSizeTransformComponentEditor : UnityEditor.Editor
    {
        private ScreenSizeTransformComponent _orientationTransform;

        private void OnEnable()
        {
            _orientationTransform = target as ScreenSizeTransformComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("DetectSize")) DetectSize();
            GUILayout.EndHorizontal();
            CreateButtons();
        }
        
        private void CreateButtons()
        {
            GUILayout.BeginHorizontal();
            var values = Enum.GetValues(typeof(ScreenSizeType))
                .Cast<ScreenSizeType>();
            foreach (var value in values)
            {
                if (GUILayout.Button($"{value.ToString()}")) MakeAndResolution(value);
            }
            GUILayout.EndHorizontal();
        }
        
        private void MakeAndResolution(ScreenSizeType sizeType)
        {
            var resolution = DeviceService.GetDefaultScreenSizeResolution(sizeType);
            PlayModeWindow.SetCustomRenderingResolution(
                (uint)resolution.Item1, 
                (uint)resolution.Item2, 
                sizeType.ToString());
            MakeAll(sizeType);
        }

        private void DetectSize()
        {
            var screenSize = DeviceService.GetScreenSizeType(
                Camera.main.pixelWidth, Camera.main.pixelHeight);
            MakeAll(screenSize);
        }

        private void MakeAll(ScreenSizeType sizeType)
        {
            var findRoot = _orientationTransform.transform.parent;
            while (findRoot.parent != null)
            {
                findRoot = findRoot.parent;
            }
            
            var allScreenSizeTransforms = 
                findRoot.GetComponentsInChildren<ScreenSizeTransformComponent>();
            foreach (var screenSizeTransform in allScreenSizeTransforms)
            {
                screenSizeTransform.SetOrientation(sizeType);
            }
        }
    }
}
#endif