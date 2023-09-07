using UnityEngine;

namespace Common.UI.SafeArea
{
    [RequireComponent(typeof(Canvas))]
    public class SafeAreaCanvas : MonoBehaviour
    {
        [SerializeField]
        private RectTransform safeAreaRect;

        private Rect _lastSafeArea = Rect.zero;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                _lastSafeArea = Screen.safeArea;
                ApplySafeArea();
            }
        }

        private void Start()
        {
            _lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            var pixelRect = _canvas.pixelRect;
            anchorMin.x /= pixelRect.width;
            anchorMin.y /= pixelRect.height;
            anchorMax.x /= pixelRect.width;
            anchorMax.y /= pixelRect.height;

            safeAreaRect.anchorMin = anchorMin;
            safeAreaRect.anchorMax = anchorMax;
        }
    }
}