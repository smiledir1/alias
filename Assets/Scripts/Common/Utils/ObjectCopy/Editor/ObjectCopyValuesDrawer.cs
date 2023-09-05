#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Utils.ObjectCopy.Editor
{
    [CustomPropertyDrawer(typeof(ObjectCopyValues), true)]
    public class ObjectCopyValuesDrawer : PropertyDrawer
    {
        private readonly float _lineHeight = 20f;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            DrawElements(rect, property);
            EditorGUI.PropertyField(rect, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label);

        private void DrawElements(Rect propertyRect, SerializedProperty property)
        {
            var usePart = propertyRect.width / 3;
            var width = 2 * usePart / 3;
            var x = propertyRect.x + usePart;
            var y = propertyRect.yMin;
            var buttonRect = new Rect(x, y, width, _lineHeight);
            if (GUI.Button(buttonRect, "Copy from RT")) GetTransform(property);

            x += width;
            buttonRect = new Rect(x, y, width, _lineHeight);
            if (GUI.Button(buttonRect, "Paste to RT")) SetTransform(property);

            x += width;
            var objectRect = new Rect(x, y, width, _lineHeight);
            var rectTransform = EditorGUI.ObjectField(
                objectRect, null, typeof(RectTransform), true) as RectTransform;
            if (rectTransform != null) CopyTransform(property, rectTransform);
        }

        private void GetTransform(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            if (targetObject is Component component)
            {
                var rectTransform = component.GetComponent<RectTransform>();
                CopyTransform(property, rectTransform);
            }
        }

        private void SetTransform(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            if (targetObject is Component component)
            {
                var rectTransform = component.GetComponent<RectTransform>();
                PasteTransform(property, rectTransform);
            }
        }

        private void CopyTransform(SerializedProperty property, RectTransform rectTransform)
        {
            var localPositionProperty = property.FindPropertyRelative("_localPosition");
            localPositionProperty.vector3Value = rectTransform.anchoredPosition3D;

            var localRotationProperty = property.FindPropertyRelative("_localRotation");
            localRotationProperty.vector3Value = rectTransform.eulerAngles;

            var localScaleProperty = property.FindPropertyRelative("_localScale");
            localScaleProperty.vector3Value = rectTransform.localScale;

            var anchorMaxProperty = property.FindPropertyRelative("_anchorMax");
            anchorMaxProperty.vector2Value = rectTransform.anchorMax;

            var anchorMinProperty = property.FindPropertyRelative("_anchorMin");
            anchorMinProperty.vector2Value = rectTransform.anchorMin;

            var pivotProperty = property.FindPropertyRelative("_pivot");
            pivotProperty.vector2Value = rectTransform.pivot;

            var sizeDeltaProperty = property.FindPropertyRelative("_sizeDelta");
            sizeDeltaProperty.vector2Value = rectTransform.sizeDelta;

            var text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                if (text.enableAutoSizing)
                {
                    var fontSizeMaxProperty = property.FindPropertyRelative("_fontSizeMax");
                    var fontSizeMinProperty = property.FindPropertyRelative("_fontSizeMin");
                    fontSizeMaxProperty.floatValue = text.fontSizeMax;
                    fontSizeMinProperty.floatValue = text.fontSizeMin;
                }
                else
                {
                    var fontSizeProperty = property.FindPropertyRelative("_fontSize");
                    fontSizeProperty.floatValue = text.fontSize;
                }
            }

            var image = rectTransform.GetComponent<Image>();
            if (image != null)
            {
                var raycastTargetProperty = property.FindPropertyRelative("_raycastTarget");
                var pixelsPerUnitMultiplierProperty = property.FindPropertyRelative("_pixelsPerUnitMultiplier");
                raycastTargetProperty.boolValue = image.raycastTarget;
                pixelsPerUnitMultiplierProperty.floatValue = image.pixelsPerUnitMultiplier;
            }
        }

        private void PasteTransform(SerializedProperty property, RectTransform rectTransform)
        {
            var anchorMaxProperty = property.FindPropertyRelative("_anchorMax");
            rectTransform.anchorMax = anchorMaxProperty.vector2Value;

            var anchorMinProperty = property.FindPropertyRelative("_anchorMin");
            rectTransform.anchorMin = anchorMinProperty.vector2Value;

            var pivotProperty = property.FindPropertyRelative("_pivot");
            rectTransform.pivot = pivotProperty.vector2Value;

            var sizeDeltaProperty = property.FindPropertyRelative("_sizeDelta");
            rectTransform.sizeDelta = sizeDeltaProperty.vector2Value;

            var localPositionProperty = property.FindPropertyRelative("_localPosition");
            rectTransform.anchoredPosition3D = localPositionProperty.vector3Value;

            var localRotationProperty = property.FindPropertyRelative("_localRotation");
            rectTransform.eulerAngles = localRotationProperty.vector3Value;

            var localScaleProperty = property.FindPropertyRelative("_localScale");
            rectTransform.localScale = localScaleProperty.vector3Value;

            var text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                if (text.enableAutoSizing)
                {
                    var fontSizeMaxProperty = property.FindPropertyRelative("_fontSizeMax");
                    var fontSizeMinProperty = property.FindPropertyRelative("_fontSizeMin");
                    text.fontSizeMax = fontSizeMaxProperty.floatValue;
                    text.fontSizeMin = fontSizeMinProperty.floatValue;
                }
                else
                {
                    var fontSizeProperty = property.FindPropertyRelative("_fontSize");
                    text.fontSize = fontSizeProperty.floatValue;
                }
            }

            var image = rectTransform.GetComponent<Image>();
            if (image != null)
            {
                var raycastTargetProperty = property.FindPropertyRelative("_raycastTarget");
                var pixelsPerUnitMultiplierProperty = property.FindPropertyRelative("_pixelsPerUnitMultiplier");
                image.raycastTarget = raycastTargetProperty.boolValue;
                image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplierProperty.floatValue;
            }
        }
    }
}
#endif