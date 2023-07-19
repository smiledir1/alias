#if UNITY_EDITOR
using Services.UI.Editor;
using UnityEditor;
using UnityEngine;

namespace Services.UI.PopupService.Editor
{
    [CustomEditor(typeof(Popup), true)]
    public class PopupEditor : UIObjectEditor
    {
        private Popup _targetPopup;

        protected new void OnEnable()
        {
            _targetPopup = target as Popup;
        }
        
        public override void OnInspectorGUI()
        {
            DoDrawDefaultInspector(serializedObject);
        }
        
        private void DoDrawDefaultInspector(SerializedObject obj)
        {
            if (GUILayout.Button("Create Addressable")) CreateAddressable(_targetPopup);
            
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            var iterator = obj.GetIterator();
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    EditorGUILayout.PropertyField(iterator, true);
                if (iterator.name == "_closeButton")
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Current UI", EditorStyles.boldLabel);
                }
            }

            obj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}
#endif