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
        private Vector3 _localPosition;

        [SerializeField]
        private Vector3 _localRotation;

        [SerializeField]
        private Vector3 _localScale;

        [SerializeField]
        private Vector2 _anchorMax;

        [SerializeField]
        private Vector2 _anchorMin;

        [SerializeField]
        private Vector2 _pivot;

        [SerializeField]
        private Vector2 _sizeDelta;

        [Header("Text")]
        [SerializeField]
        private float _fontSize;

        [SerializeField]
        private float _fontSizeMax;

        [SerializeField]
        private float _fontSizeMin;


        [Header("Image")]
        [SerializeField]
        private bool _raycastTarget;

        [SerializeField]
        private float _pixelsPerUnitMultiplier;

        public ObjectCopyValues()
        {
        }

        public ObjectCopyValues(RectTransform rectTransform)
        {
            CopyFrom(rectTransform);
        }

        public void CopyFrom(RectTransform rectTransform)
        {
            _localPosition = rectTransform.anchoredPosition3D;
            _localRotation = rectTransform.eulerAngles;
            _localScale = rectTransform.localScale;
            _anchorMax = rectTransform.anchorMax;
            _anchorMin = rectTransform.anchorMin;
            _pivot = rectTransform.pivot;
            _sizeDelta = rectTransform.sizeDelta;

            var text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                if (text.enableAutoSizing)
                {
                    _fontSizeMax = text.fontSizeMax;
                    _fontSizeMin = text.fontSizeMin;
                }
                else
                    _fontSize = text.fontSize;
            }

            var image = rectTransform.GetComponent<Image>();
            if (image != null)
            {
                _raycastTarget = image.raycastTarget;
                _pixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier;
            }
        }

        public void PasteTo(RectTransform rectTransform)
        {
            rectTransform.anchorMax = _anchorMax;
            rectTransform.anchorMin = _anchorMin;
            rectTransform.pivot = _pivot;
            rectTransform.sizeDelta = _sizeDelta;
            rectTransform.anchoredPosition3D = _localPosition;
            rectTransform.eulerAngles = _localRotation;
            rectTransform.localScale = _localScale;

            var text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                if (text.enableAutoSizing)
                {
                    text.fontSizeMax = _fontSizeMax;
                    text.fontSizeMin = _fontSizeMin;
                }
                else
                    text.fontSize = _fontSize;
            }

            var image = rectTransform.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = _raycastTarget;
                image.pixelsPerUnitMultiplier = _pixelsPerUnitMultiplier;
            }
        }
    }
}