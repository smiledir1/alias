#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.Editor
{
    [CustomPropertyDrawer(typeof(SimpleTween), true)]
    public class SimpleTweenDrawer : PropertyDrawer
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

        private static SimpleTween _cachedTween;
        
        private int _buttonsCount = 4;
        private float _buttonHeight = 20f;
        private string _cachedName = String.Empty;
        private float ButtonsHeight => _buttonHeight * 2;
        private float Space => 10f;
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            string tweenName;
            if ( property.managedReferenceId == -1 ||
                 property.managedReferenceValue == null)
            {
                property.isExpanded = true;
                tweenName = "Null";
            }
            else
            {
                tweenName = property.managedReferenceValue.GetType().ToString();
            }
            
            var propertyYAdd = 0f;
            if (property.isExpanded)
            {
                DrawSetTweenButtons(rect, property);
                propertyYAdd = ButtonsHeight;
            }

            var lastIndexOfPoint = tweenName.LastIndexOf('.');
            var shortTweenNameLength = tweenName.Length - lastIndexOfPoint - 1;
            var shortTweenName = tweenName.Substring(lastIndexOfPoint + 1, shortTweenNameLength);

            if (string.Equals(_cachedName, shortTweenName, StringComparison.Ordinal))
            {
                label.text = _cachedName;
            }
            else
            {
                label.text += $" {shortTweenName}";
                _cachedName = label.text;
            }

            var propertyRect = new Rect(rect.x, rect.y + propertyYAdd, rect.width, rect.height);
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded
                ? EditorGUI.GetPropertyHeight(property) + ButtonsHeight + Space
                : EditorGUI.GetPropertyHeight(property);
        }

        private void DrawSetTweenButtons(Rect propertyRect, SerializedProperty property)
        {
            var buttonWidth = propertyRect.width / _buttonsCount;
            var y = propertyRect.yMin;

            if (_tweenObject == null)
            {
                _tweenObject = property.serializedObject.targetObject switch
                {
                    GameObject go => go,
                    Component component => component.gameObject,
                    _ => _tweenObject
                };
            }

            SimpleTween tween = null;

            var x = propertyRect.x;
            var buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Position"))
            {
                tween = new PositionTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    _animationCurve,
                    Vector3.zero,
                    Vector3.zero);
            }

            x = propertyRect.x + buttonWidth * 1;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Rotation"))
            {
                tween = new RotationTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    _animationCurve,
                    Vector3.zero,
                    Vector3.zero);
            }

            x = propertyRect.x + buttonWidth * 2;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Scale"))
            {
                tween = new ScaleTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    _animationCurve,
                    Vector3.zero,
                    Vector3.one);
            }

            x = propertyRect.x + buttonWidth * 3;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Transparency"))
            {
                var canvasGroup = _tweenObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = _tweenObject.gameObject.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 1f;
                }

                tween = new TransparencyTween(
                    _tweenObject,
                    _startDelay,
                    _tweenTime,
                    _loop,
                    _animationCurve,
                    canvasGroup,
                    0,
                    1);
            }

            y += _buttonHeight;
            x = propertyRect.x;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Null"))
            {
                property.managedReferenceValue = null;
            }
            
            x = propertyRect.x + buttonWidth * 1;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Copy"))
            {
                _cachedTween = property.managedReferenceValue as SimpleTween;
            }
            
            x = propertyRect.x + buttonWidth * 2;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Paste"))
            {
                var currentTween = property.managedReferenceValue as SimpleTween;
                var targetGo =  currentTween?.TweenObject;
                if (targetGo == null)
                {
                    var component = property.serializedObject?.targetObject as Component;
                    if(component != null)targetGo = component.gameObject;
                };
                var cloneTween = SimpleTween.Clone(_cachedTween, targetGo);
                property.managedReferenceValue = cloneTween;
            }

            if (tween != null)
            {
                if (property.managedReferenceId == -1)
                {
                    Debug.Log("Shoud Be SerializeReference");
                }
                else
                {
                    property.managedReferenceValue = tween;
                }
            }
        }
    }
}
#endif