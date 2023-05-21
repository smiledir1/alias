#if UNITY_EDITOR
using UnityEditor;

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
    }
}
#endif