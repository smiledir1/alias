#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Services.Device.Components.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OrientationTransformComponent), true)]
    public class OrientationTransformComponentEditor : UnityEditor.Editor
    {
        private OrientationTransformComponent _orientationTransform;

        private void OnEnable()
        {
            _orientationTransform = target as OrientationTransformComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            CreateButtons();
            if (GUILayout.Button("DetectSize")) DetectSize();
            GUILayout.EndHorizontal();
        }

        private void CreateButtons()
        {
            var values = Enum.GetValues(typeof(ScreenSimpleOrientation))
                .Cast<ScreenSimpleOrientation>();
            foreach (var value in values)
            {
                if (GUILayout.Button($"Make All {value.ToString()}")) MakeAndResolution(value);
            }
        }
        
        private void DetectSize()
        {
            var main = Camera.main;
            if (main == null) return;
            var orientation = DeviceService.GetSimpleOrientation(
                main.pixelWidth, main.pixelHeight);
            MakeAll(orientation);
        }
        
        private void MakeAll(ScreenSimpleOrientation orientation)
        {
            var findRoot = _orientationTransform.transform.parent;
            while (findRoot.parent != null)
            {
                findRoot = findRoot.parent;
            }
            
            var allOrientationTransforms = 
                findRoot.GetComponentsInChildren<OrientationTransformComponent>();
            foreach (var orientationTransform in allOrientationTransforms)
            {
                orientationTransform.SetOrientation(orientation);
            }
        }

        private void MakeAndResolution(ScreenSimpleOrientation orientation)
        {
            var resolution = DeviceService.GetDefaultOrientationResolution(orientation);
            PlayModeWindow.SetCustomRenderingResolution(
                (uint)resolution.Item1, 
                (uint)resolution.Item2, 
                orientation.ToString());
            MakeAll(orientation);
        }
    }
}
#endif