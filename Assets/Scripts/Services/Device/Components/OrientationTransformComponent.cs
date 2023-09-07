using System;
using Common.Utils.ObjectCopy;
using Services.Helper;
using UnityEngine;

namespace Services.Device.Components
{
    public class OrientationTransformComponent : MonoBehaviour
    {
        [SerializeField]
        private bool initializeOnAwake = true;

        [SerializeField]
        private ObjectCopyValues portraitObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues landscapeObjectCopyValues;

        [Service]
        private static IDeviceService _deviceService;

        private void Awake()
        {
            if (initializeOnAwake) Initialize();
        }

        private void OnDestroy()
        {
            _deviceService.OnSizeChange -= OnSizeChange;
        }

        public void Initialize()
        {
            _deviceService.OnSizeChange -= OnSizeChange;
            _deviceService.OnSizeChange += OnSizeChange;
            SetOrientation(_deviceService.CurrentScreenSimpleOrientation);
        }

        public void SetOrientation(ScreenSimpleOrientation orientation)
        {
            if (transform is not RectTransform rectTransform) return;
            switch (orientation)
            {
                case ScreenSimpleOrientation.Portrait:
                    portraitObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSimpleOrientation.Landscape:
                    landscapeObjectCopyValues.PasteTo(rectTransform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orientation), orientation, null);
            }
        }

        private void OnSizeChange(int width, int height)
        {
            SetOrientation(_deviceService.CurrentScreenSimpleOrientation);
        }
    }
}