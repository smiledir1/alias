using Common.UniTaskAnimations.Editor;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens.Editor
{
    [CustomPropertyDrawer(typeof(BezierPositionTween), true)]
    public class BezierPositionTweenDrawer : SimpleTweenDrawer
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
            
            var bezier1OffsetRect = new Rect(x, y, vectorWidth, height);
            var bezier1OffsetProperty = property.FindPropertyRelative("_bezier1Offset");
            EditorGUI.PropertyField(bezier1OffsetRect, bezier1OffsetProperty);
            
            var bezier1OffsetGoToButtonRect = new Rect(buttonX, y, buttonWidth, height);
            if (GUI.Button(bezier1OffsetGoToButtonRect, "Go To")) Bezier1OffsetGoToPosition();
            
            var bezier1OffsetCopyButtonRect = new Rect(buttonX2, y, buttonWidth, height);
            if (GUI.Button(bezier1OffsetCopyButtonRect, "Copy From OBJ")) Bezier1OffsetCopyPosition();
            y += height;
            
            var bezier2OffsetRect = new Rect(x, y, vectorWidth, height);
            var bezier2OffsetProperty = property.FindPropertyRelative("_bezier2Offset");
            EditorGUI.PropertyField(bezier2OffsetRect, bezier2OffsetProperty);
            
            var bezier2OffsetGoToButtonRect = new Rect(buttonX, y, buttonWidth, height);
            if (GUI.Button(bezier2OffsetGoToButtonRect, "Go To")) Bezier2OffsetGoToPosition();
            
            var bezier2OffsetCopyButtonRect = new Rect(buttonX2, y, buttonWidth, height);
            if (GUI.Button(bezier2OffsetCopyButtonRect, "Copy From OBJ")) Bezier2OffsetCopyPosition();
            y += height;

            var precisionRect = new Rect(x, y, vectorWidth, height);
            var precisionProperty = property.FindPropertyRelative("_precision");
            EditorGUI.PropertyField(precisionRect, precisionProperty);
            y += height;
            
            return y - propertyRect.y;
        }

        private void FromGoToPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            bezierPositionTween.GoToPosition(bezierPositionTween.FromPosition);
        }

        private void FromCopyPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            var fromPosition = bezierPositionTween.GetCurrentPosition();
            bezierPositionTween.SetPositions(
                bezierPositionTween.PositionType,
                fromPosition, 
                bezierPositionTween.ToPosition,
                bezierPositionTween.Bezier1Offset,
                bezierPositionTween.Bezier2Offset);
        }

        private void ToGoToPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            bezierPositionTween.GoToPosition(bezierPositionTween.ToPosition);
        }

        private void ToCopyPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            var toPosition= bezierPositionTween.GetCurrentPosition();
            bezierPositionTween.SetPositions(
                bezierPositionTween.PositionType,
                bezierPositionTween.FromPosition, 
                toPosition,
                bezierPositionTween.Bezier1Offset,
                bezierPositionTween.Bezier2Offset);
        }

        private void Bezier1OffsetGoToPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            var position = bezierPositionTween.Bezier1Offset + bezierPositionTween.FromPosition;
            bezierPositionTween.GoToPosition(position);
        }

        private void Bezier1OffsetCopyPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            var position= bezierPositionTween.GetCurrentPosition();
            var bezierOffset = position - bezierPositionTween.FromPosition;
            bezierPositionTween.SetPositions(
                bezierPositionTween.PositionType,
                bezierPositionTween.FromPosition, 
                bezierPositionTween.ToPosition,
                bezierOffset,
                bezierPositionTween.Bezier2Offset);
        }

        private void Bezier2OffsetGoToPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            var position = bezierPositionTween.Bezier2Offset + bezierPositionTween.ToPosition;
            bezierPositionTween.GoToPosition(position);
        }

        private void Bezier2OffsetCopyPosition()
        {
            if (TargetTween is not BezierPositionTween bezierPositionTween) return;
            var position= bezierPositionTween.GetCurrentPosition();
            var bezierOffset = position - bezierPositionTween.ToPosition;
            bezierPositionTween.SetPositions(
                bezierPositionTween.PositionType,
                bezierPositionTween.FromPosition, 
                bezierPositionTween.ToPosition,
                bezierPositionTween.Bezier1Offset,
                bezierOffset);
        }
    }
}