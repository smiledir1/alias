using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Services.Common;
using UnityEngine;

namespace Services.Device
{
    public class DeviceService : Service, IDeviceService
    {
        public event Action<ScreenSizeType> ScreenSizeTypeChange;
        public event Action<ScreenSimpleOrientation> OrientationChange;
        public event Action<int, int> OnSizeChange;

        public ScreenSimpleOrientation CurrentScreenSimpleOrientation { get; private set; }
        public ScreenSizeType CurrentScreenSizeType { get; private set; }

        private int _lastWidth;
        private int _lastHeight;

        protected override UniTask OnInitialize()
        {
            Update().Forget();
            CurrentScreenSimpleOrientation = GetSimpleOrientation();
            return base.OnInitialize();
        }

        private async UniTask Update(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(cancellationToken);

                var sizeChange = false;
                var frameOrientationChange = false;
                var screenSizeTypeChange = false;

                if (_lastWidth != Screen.width ||
                    _lastHeight != Screen.height)
                {
                    _lastWidth = Screen.width;
                    _lastHeight = Screen.height;
                    sizeChange = true;
                }

                if (!sizeChange) continue;

                var frameOrientation = GetSimpleOrientation();
                if (frameOrientation != CurrentScreenSimpleOrientation)
                {
                    CurrentScreenSimpleOrientation = frameOrientation;
                    frameOrientationChange = true;
                }

                var screenSizeType = GetScreenSizeType();
                if (screenSizeType != CurrentScreenSizeType)
                {
                    CurrentScreenSizeType = screenSizeType;
                    screenSizeTypeChange = true;
                }

                OnSizeChange?.Invoke(_lastWidth, _lastHeight);
                if (frameOrientationChange) OrientationChange?.Invoke(frameOrientation);
                if (screenSizeTypeChange) ScreenSizeTypeChange?.Invoke(screenSizeType);
            }
        }

        private ScreenSimpleOrientation GetSimpleOrientation() =>
            GetSimpleOrientation(_lastWidth, _lastHeight);

        private ScreenSizeType GetScreenSizeType() =>
            GetScreenSizeType(_lastWidth, _lastHeight);

        public static ScreenSimpleOrientation GetSimpleOrientation(float width, float height) =>
            width < height
                ? ScreenSimpleOrientation.Portrait
                : ScreenSimpleOrientation.Landscape;

        public static ScreenSizeType GetScreenSizeType(float width, float height)
        {
            // 2.1641 - LandscapeIphone
            // 1.7777 - 16:9 - landscape
            // 1.3333 - 3:4 - ipad
            // 0.7500 - 4:3 - ipad
            // 0.5625 - 9:16 - portrait
            // 0.4737 - 9:19
            // 0.4615 - 9:19.5
            // 0.4615 - PortraitIphone
            // 0.4500 - 9:20
            var aspect = width / height;

            if (aspect < 0.45f) return ScreenSizeType.ExtraThin;

            if (aspect >= 0.45f && aspect < 0.55f) return ScreenSizeType.PortraitIphone;

            if (aspect >= 0.55f && aspect < 0.73f) return ScreenSizeType.Portrait;

            if (aspect >= 0.73f && aspect <= 1f) return ScreenSizeType.PortraitTablet;

            if (aspect > 1f && aspect <= 1.35f) return ScreenSizeType.LandscapeTablet;

            if (aspect > 1.35f && aspect <= 1.79f) return ScreenSizeType.Landscape;

            if (aspect > 1.79f && aspect <= 2.18f) return ScreenSizeType.LandscapeIphone;

            if (aspect > 2.18f) return ScreenSizeType.ExtraLarge;

            return aspect < 1f ? ScreenSizeType.ExtraThin : ScreenSizeType.ExtraLarge;
        }

        public static (int, int) GetDefaultOrientationResolution(ScreenSimpleOrientation orientation)
        {
            return orientation switch
            {
                ScreenSimpleOrientation.Portrait => (1080, 1920),
                ScreenSimpleOrientation.Landscape => (1920, 1080),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(orientation), orientation, null)
            };
        }

        public static (int, int) GetDefaultScreenSizeResolution(ScreenSizeType orientation)
        {
            return orientation switch
            {
                ScreenSizeType.ExtraThin => (720, 1600),
                ScreenSizeType.PortraitIphone => (1170, 2532),
                ScreenSizeType.Portrait => (1080, 1920),
                ScreenSizeType.PortraitTablet => (1536, 2048),
                ScreenSizeType.LandscapeTablet => (2048, 1536),
                ScreenSizeType.Landscape => (1920, 1080),
                ScreenSizeType.LandscapeIphone => (2532, 1170),
                ScreenSizeType.ExtraLarge => (1600, 720),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(orientation), orientation, null)
            };
        }
    }

    public enum ScreenSimpleOrientation
    {
        Portrait,
        Landscape
    }

    public enum ScreenSizeType
    {
        ExtraThin,
        PortraitIphone,
        Portrait,
        PortraitTablet,
        LandscapeTablet,
        Landscape,
        LandscapeIphone,
        ExtraLarge
    }
}