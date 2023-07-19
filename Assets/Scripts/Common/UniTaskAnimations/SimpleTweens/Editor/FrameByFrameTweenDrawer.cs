#if UNITY_EDITOR
using Common.UniTaskAnimations.Editor;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens.Editor
{
    [CustomPropertyDrawer(typeof(FrameByFrameTween), true)]
    public class FrameByFrameTweenDrawer : SimpleTweenDrawer
    {
        private string _textFieldValue;
        private int _framesCount;
        private bool _expand;
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(rect, property, label);
            
            DrawCalculate(rect, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = base.GetPropertyHeight(property, label);
            return baseHeight + _buttonHeight;
        }

        private void DrawCalculate(Rect propertyRect, SerializedProperty property, GUIContent label)
        {
            var width = propertyRect.width / 3;
            var x = propertyRect.x;
            var y = base.GetPropertyHeight(property, label) + propertyRect.yMin;

            var labelRect = new Rect(x, y, width, _buttonHeight);
            GUI.Label(labelRect, "time to: frames in second (1 second)");

            x += width;
            var textFieldRect = new Rect(x, y, width, _buttonHeight);
            _textFieldValue = GUI.TextField(textFieldRect, _textFieldValue);
            if (!int.TryParse(_textFieldValue, out var value) ||
                value < 1)
            {
                _textFieldValue = "1";
                _framesCount = 1;
            }
            else
            {
                _framesCount = value;
            }

            x += width;
            var buttonRect = new Rect(x, y, width, _buttonHeight);
            if (GUI.Button(buttonRect, "Calculate"))
            {
                if (property.managedReferenceValue is FrameByFrameTween currentTween)
                {
                    var newTime = currentTween.Sprites.Count / (float) _framesCount;
                    property.managedReferenceValue = new FrameByFrameTween(
                        currentTween.TweenObject,
                        currentTween.StartDelay,
                        newTime,
                        currentTween.Loop,
                        currentTween.AnimationCurve,
                        currentTween.TweenImage,
                        currentTween.Sprites);
                }
            }
        }
    }
}
#endif