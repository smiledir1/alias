#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.Editor
{
    [CustomEditor(typeof(GroupTweenComponent), true)]
    public class GroupTweenComponentEditor : UnityEditor.Editor
    {
        #region Example Values

        private GameObject _tweenObject;
        private readonly float _startDelay = 0f;
        private readonly float _tweenTime = 1f;
        private readonly LoopType _loop = LoopType.Once;

        private readonly AnimationCurve _animationCurve = new()
        {
            keys = new[] {new Keyframe(0, 0), new Keyframe(1, 1)}
        };

        #endregion /Example Values
        
        private GroupTweenComponent _target;

        protected void OnEnable()
        {
            _target = target as GroupTweenComponent;
            if (_target != null) _tweenObject = _target.gameObject;
        }

        public override void OnInspectorGUI()
        {
            DrawAddButtons();
            DrawAnimationButtons();
            base.OnInspectorGUI();
        }

        private void DrawAddButtons()
        {
            SimpleTween tween = null;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Position Tween"))
            {
                tween = new PositionTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    CloneAnimationCurve(),
                    Vector3.zero,
                    Vector3.zero);
            }

            if (GUILayout.Button("Add Rotation Tween"))
            {
                tween = new RotationTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    CloneAnimationCurve(),
                    Vector3.zero,
                    Vector3.zero);
            }

            if (GUILayout.Button("Add Scale Tween"))
            {
                tween = new ScaleTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    CloneAnimationCurve(),
                    Vector3.zero,
                    Vector3.one);
            }

            if (GUILayout.Button("Add Transparency Tween"))
            {
                var canvasGroup = _target.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = _target.gameObject.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 1f;
                }

                tween = new TransparencyTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    CloneAnimationCurve(),
                    canvasGroup,
                    0,
                    1);
            }

            EditorGUILayout.EndHorizontal();

            if (tween != null)
            {
                _target.AddTween(tween);
            }
        }

        private void DrawAnimationButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start All (Reset)"))
            {
                var allAnimations = _target.Tweens;
                foreach (var animation in allAnimations)
                {
                    animation.StartAnimation().Forget();
                }
            }

            if (GUILayout.Button("Stop All"))
            {
                var allAnimations = _target.Tweens;
                foreach (var animation in allAnimations)
                {
                    animation.StopAnimation();
                }
            }

            if (GUILayout.Button("Reset All"))
            {
                var allAnimations = _target.Tweens;
                foreach (var animation in allAnimations)
                {
                    animation.ResetValues();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private AnimationCurve CloneAnimationCurve()
        {
            var curve = new AnimationCurve();
            foreach (var key in _animationCurve.keys)
            {
                curve.AddKey(key.time, key.value);
            }

            return curve;
        }
    }
}
#endif
