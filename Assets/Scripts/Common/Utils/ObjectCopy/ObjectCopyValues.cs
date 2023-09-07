using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Utils.ObjectCopy
{
    [Serializable]
    public class ObjectCopyValues
    {
        [Header("Transform")]
        [SerializeField]
        private Vector3 localPosition;

        [SerializeField]
        private Vector3 localRotation;

        [SerializeField]
        private Vector3 localScale;

        [SerializeField]
        private Vector2 anchorMax;

        [SerializeField]
        private Vector2 anchorMin;

        [SerializeField]
        private Vector2 pivot;

        [SerializeField]
        private Vector2 sizeDelta;

        [Header("Text")]
        [SerializeField]
        private float fontSize;

        [SerializeField]
        private float fontSizeMax;

        [SerializeField]
        private float fontSizeMin;


        [Header("Image")]
        [SerializeField]
        private bool raycastTarget;

        [SerializeField]
        private float pixelsPerUnitMultiplier;

        public ObjectCopyValues()
        {
        }

        public ObjectCopyValues(RectTransform rectTransform)
        {
            CopyFrom(rectTransform);
        }

        public void CopyFrom(RectTransform rectTransform)
        {
            localPosition = rectTransform.anchoredPosition3D;
            localRotation = rectTransform.eulerAngles;
            localScale = rectTransform.localScale;
            anchorMax = rectTransform.anchorMax;
            anchorMin = rectTransform.anchorMin;
            pivot = rectTransform.pivot;
            sizeDelta = rectTransform.sizeDelta;

            var text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                if (text.enableAutoSizing)
                {
                    fontSizeMax = text.fontSizeMax;
                    fontSizeMin = text.fontSizeMin;
                }
                else
                {
                    fontSize = text.fontSize;
                }
            }

            var image = rectTransform.GetComponent<Image>();
            if (image != null)
            {
                raycastTarget = image.raycastTarget;
                pixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier;
            }
        }

        public void PasteTo(RectTransform rectTransform)
        {
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchorMin = anchorMin;
            rectTransform.pivot = pivot;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition3D = localPosition;
            rectTransform.eulerAngles = localRotation;
            rectTransform.localScale = localScale;

            var text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                if (text.enableAutoSizing)
                {
                    text.fontSizeMax = fontSizeMax;
                    text.fontSizeMin = fontSizeMin;
                }
                else
                {
                    text.fontSize = fontSize;
                }
            }

            var image = rectTransform.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = raycastTarget;
                image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
            }
        }
    }
}