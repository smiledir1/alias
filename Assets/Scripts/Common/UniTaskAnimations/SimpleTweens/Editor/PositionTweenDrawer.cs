using Common.UniTaskAnimations.Editor;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens.Editor
{
    [CustomPropertyDrawer(typeof(PositionTween), true)]
    public class PositionTweenDrawer : SimpleTweenDrawer
    {
        protected override float DrawTweenProperties(
            Rect propertyRect,
            SerializedProperty property,
            GUIContent label)
        {
            var x = propertyRect.x;
            var y = propertyRect.y;
            var width = propertyRect.width;
            var height = _lineHeight;

            var vectorWidth = width * 2 / 3;
            var buttonWidth = width / 6;

            var labelRect = new Rect(x, y, width, height);
            EditorGUI.LabelField(labelRect, "Current Tween", EditorStyles.boldLabel);
            y += height;
            
            var positionTypeRect = new Rect(x, y, width, height);
            var positionTypeProperty = property.FindPropertyRelative("_positionType");
            EditorGUI.PropertyField(positionTypeRect, positionTypeProperty);
            y += height;
            
            var fromPositionRect = new Rect(x, y, vectorWidth, height);
            var fromPositionProperty = property.FindPropertyRelative("_fromPosition");
            EditorGUI.PropertyField(fromPositionRect, fromPositionProperty);

            var buttonX = x + vectorWidth;
            var fromGoToButtonRect = new Rect(buttonX, y, buttonWidth, height);
            if (GUI.Button(fromGoToButtonRect, "Go To")) FromGoToPosition();
            
            var buttonX2 = buttonX + buttonWidth;
            var fromCopyButtonRect = new Rect(buttonX2, y, buttonWidth, height);
            if (GUI.Button(fromCopyButtonRect, "Copy From OBJ")) FromCopyPosition();
            y += height;

            var toPositionRect = new Rect(x, y, vectorWidth, height);
            var toPositionProperty = property.FindPropertyRelative("_toPosition");
            EditorGUI.PropertyField(toPositionRect, toPositionProperty);
            
            var toGoToButtonRect = new Rect(buttonX, y, buttonWidth, height);
            if (GUI.Button(toGoToButtonRect, "Go To")) ToGoToPosition();
            
            var toCopyButtonRect = new Rect(buttonX2, y, buttonWidth, height);
            if (GUI.Button(toCopyButtonRect, "Copy From OBJ")) ToCopyPosition();
            y += height;


            return y - propertyRect.y;
        }

        private void FromGoToPosition()
        {
            if (TargetTween is not PositionTween positionTween) return;
            positionTween.GoToPosition(positionTween.FromPosition);
        }

        private void FromCopyPosition()
        {
            if (TargetTween is not PositionTween positionTween) return;
            var fromPosition = positionTween.GetCurrentPosition();
            positionTween.SetPositions(
                fromPosition, 
                positionTween.ToPosition,
                positionTween.PositionType);
        }

        private void ToGoToPosition()
        {
            if (TargetTween is not PositionTween positionTween) return;
            positionTween.GoToPosition(positionTween.ToPosition);
        }

        private void ToCopyPosition()
        {
            if (TargetTween is not PositionTween positionTween) return;
            var toPosition= positionTween.GetCurrentPosition();
            positionTween.SetPositions(
                positionTween.FromPosition, 
                toPosition,
                positionTween.PositionType);
        }
    }
}