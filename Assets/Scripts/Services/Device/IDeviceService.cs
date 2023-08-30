using System;
using Services.Common;

namespace Services.Device
{
    public interface IDeviceService : IService
    {
        event Action<ScreenSimpleOrientation> OrientationChange;
        ScreenSimpleOrientation CurrentScreenSimpleOrientation { get; }
        ScreenSizeType CurrentScreenSizeType { get; }
        event Action<int, int> OnSizeChange;
        event Action<ScreenSizeType> ScreenSizeTypeChange;
    }
}