using System;
using Services.Device;
using Services.Helper;
using UnityEngine;

namespace Common.Utils.ObjectCopy.Components
{
    public class ScreenSizeTransformComponent : MonoBehaviour
    {
        [SerializeField]
        private bool initializeOnAwake = true;

        [SerializeField]
        private ObjectCopyValues extraThinObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues portraitIphoneObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues portraitObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues portraitTabletObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues landscapeTabletObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues landscapeObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues landscapeIphoneObjectCopyValues;

        [SerializeField]
        private ObjectCopyValues extraLargeObjectCopyValues;

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
            SetOrientation(_deviceService.CurrentScreenSizeType);
        }

        public void SetOrientation(ScreenSizeType sizeType)
        {
            if (transform is not RectTransform rectTransform) return;
            switch (sizeType)
            {
                case ScreenSizeType.ExtraThin:
                    extraThinObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.PortraitIphone:
                    portraitIphoneObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.Portrait:
                    portraitObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.PortraitTablet:
                    portraitTabletObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.LandscapeTablet:
                    landscapeTabletObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.Landscape:
                    landscapeObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.LandscapeIphone:
                    landscapeIphoneObjectCopyValues.PasteTo(rectTransform);
                    break;
                case ScreenSizeType.ExtraLarge:
                    extraLargeObjectCopyValues.PasteTo(rectTransform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(sizeType), sizeType, null);
            }
        }

        private void OnSizeChange(int width, int height)
        {
            SetOrientation(_deviceService.CurrentScreenSizeType);
        }
    }
}