using System;
using Cysharp.Threading.Tasks;
using Services.Common;

namespace Services.UI
{
    public interface IUIService : IService
    {
        event Action<UIObject> UIObjectOpen;
        event Action<UIObject> UIObjectClose;
        event Action<UIObject> UIObjectStart;
        event Action<UIObject> UIObjectStop;
        event Action<UIObject> UIObjectLoad;
        event Action<UIObject> UIObjectUnload;

        UniTask<T> ShowAsync<T>(
            UIModel uiModel,
            bool closePreviousUI = false,
            int group = default) where T : UIObject;

        UniTask CloseCurrentUIObject();
        UniTask CloseCurrentUIObject<T>() where T : UIObject;
        UniTask CloseCurrentUIObject(UIObject uiObject);
        UniTask CloseGroupUIObjects(int group);
        UniTask UnloadAll();
        UniTask<T> LoadUIObject<T>() where T : UIObject;
        UniTask UnLoadUIObject<T>() where T : UIObject;
    }
}