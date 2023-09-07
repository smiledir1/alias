using UnityEngine;

namespace Common.UI.SafeArea
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        [SerializeField]
        private SafeAreaMode mode;

        private Rect _safeArea;

        private void UpdateRectTransform()
        {
            var rectTransform = (RectTransform) transform;
            var xMin = mode == SafeAreaMode.VerticalSafe ? 0f : _safeArea.xMin / Screen.width;
            var xMax = mode == SafeAreaMode.VerticalSafe ? 1f : _safeArea.xMax / Screen.width;

            var yMin = mode == SafeAreaMode.HorizontalSafe ? 0f : _safeArea.yMin / Screen.height;
            var yMax = mode == SafeAreaMode.HorizontalSafe ? 1f : _safeArea.yMax / Screen.height;

            rectTransform.anchorMin = new Vector2(xMin, yMin);
            rectTransform.anchorMax = new Vector2(xMax, yMax);
        }

        #region Unity

        private void Awake()
        {
            _safeArea = Screen.safeArea;

            UpdateRectTransform();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateRectTransform();
        }

        private void Update()
        {
            if (_safeArea == Screen.safeArea) return;

            _safeArea = Screen.safeArea;

            UpdateRectTransform();
        }

        #endregion
    }

    public enum SafeAreaMode
    {
        Default,
        HorizontalSafe,
        VerticalSafe
    }
}