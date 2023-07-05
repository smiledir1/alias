#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.Editor
{
    [CustomEditor(typeof(TweenComponent), true)]
    public class TweenComponentEditor : UnityEditor.Editor
    {
        private TweenComponent _target;

        protected void OnEnable()
        {
            _target = target as TweenComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawAnimationButtons();
        }

        private void DrawAnimationButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start (Reset)"))
            {
                _target.Tween.StartAnimation().Forget();
            }

            if (GUILayout.Button("Start (Current)"))
            {
                _target.Tween.StartAnimation(false, true).Forget();
            }

            if (GUILayout.Button("Start (Reverse)"))
            {
                _target.Tween.StartAnimation(true).Forget();
            }

            if (GUILayout.Button("Stop"))
            {
                _target.Tween.StopAnimation();
            }

            if (GUILayout.Button("Reset"))
            {
                _target.Tween.ResetValues();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start All (Reset)"))
            {
                var allAnimations = _target.GetComponentsInChildren<TweenComponent>();
                foreach (var animation in allAnimations)
                {
                    animation.Tween.StartAnimation().Forget();
                }
            }

            if (GUILayout.Button("Stop All"))
            {
                var allAnimations = _target.GetComponentsInChildren<TweenComponent>();
                foreach (var animation in allAnimations)
                {
                    animation.Tween.StopAnimation();
                }
            }

            if (GUILayout.Button("Reset All"))
            {
                var allAnimations = _target.GetComponentsInChildren<TweenComponent>();
                foreach (var animation in allAnimations)
                {
                    animation.Tween.ResetValues();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif