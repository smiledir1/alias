using Common.UniTaskAnimations.Editor;
using UnityEditor;
using UnityEngine;


namespace Common.UniTaskAnimations.SimpleTweens.Editor
{
    [CustomPropertyDrawer(typeof(MultiPositionTween), true)]
    public class MultiPositionTweenDrawer : SimpleTweenDrawer
    {
        protected override float DrawTweenProperties(
            Rect propertyRect,
            SerializedProperty property,
            GUIContent label)
        {
            var x = propertyRect.x;
            var y = propertyRect.y;
            var width = propertyRect.width;
            var height = LineHeight;
            
            var positionTypeRect = new Rect(x, y, width, height);
            var positionTypeProperty = property.FindPropertyRelative("_positionType");
            EditorGUI.PropertyField(positionTypeRect, positionTypeProperty);
            y += height;
            
            var lineTypeRect = new Rect(x, y, width, height);
            var lineTypeProperty = property.FindPropertyRelative("_lineType");
            EditorGUI.PropertyField(lineTypeRect, lineTypeProperty);
            y += height;
            
            var positionsRect = new Rect(x, y, width, height);
            var positionsProperty = property.FindPropertyRelative("_positions");
            EditorGUI.PropertyField(positionsRect, positionsProperty);
            y += EditorGUI.GetPropertyHeight(positionsProperty);
            
            var addCurrentPositionButtonRect = new Rect(x, y, width, height);
            if (GUI.Button(addCurrentPositionButtonRect, "Add Current Position"))
            {
                AddCurrentPosition();
            }
            y += height;

            if (TargetTween is MultiPositionTween multiPositionTween &&
                multiPositionTween.LineType != MultiLineType.Line)
            {
                var precisionRect = new Rect(x, y, width, height);
                var precisionProperty = property.FindPropertyRelative("_precision");
                EditorGUI.PropertyField(precisionRect, precisionProperty);
                y += height;
                
                var alphaRect = new Rect(x, y, width, height);
                var alphaProperty = property.FindPropertyRelative("_alpha");
                EditorGUI.PropertyField(alphaRect, alphaProperty);
                y += height;
            }

            return y - propertyRect.y;
        }

        private void AddCurrentPosition()
        {
            if (TargetTween is not MultiPositionTween multiPositionTween) return;
            var position = multiPositionTween.GetCurrentPosition();
            multiPositionTween.PointsPositions.Add(position);
        }
    }
}