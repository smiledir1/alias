#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Services.UI.Editor
{
    [CustomEditor(typeof(UIObject), true)]
    public class UIObjectEditor : UnityEditor.Editor
    {
        private UIObject _target;

        protected void OnEnable()
        {
            _target = target as UIObject;
        }

        public override void OnInspectorGUI()
        {
            DoDrawDefaultInspector(serializedObject);
        }

        private void DoDrawDefaultInspector(SerializedObject obj)
        {
            if (GUILayout.Button("Create Addressable") 
                && !Application.isPlaying) CreateAddressable(_target);

            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            var iterator = obj.GetIterator();
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    EditorGUILayout.PropertyField(iterator, true);
                if (iterator.name == "_closeTween")
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Current UI", EditorStyles.boldLabel);
                }
            }

            obj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        protected static void CreateAddressable(UIObject target)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            
            var assetPath = AssetDatabase.GetAssetPath(target);
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            settings.CreateAssetReference(assetGuid);
            var entry = settings.FindAssetEntry(assetGuid);
            if(entry == null) return;
            entry.address = target.GetType().Name;
        }
    }
}
#endif