#if UNITY_EDITOR
using UnityEditor;

namespace Services.UI.PopupService.Editor
{
    [CustomEditor(typeof(Popup), true)]
    public class PopupEditor : UnityEditor.Editor
    {
        private Popup _target;

        protected void OnEnable()
        {
            _target = target as Popup;
        }
        
        public override void OnInspectorGUI()
        {
            DoDrawDefaultInspector(serializedObject);
        }
        
        private void DoDrawDefaultInspector(SerializedObject obj)
        {
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