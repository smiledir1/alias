using Services.Audio.Components;
using Services.UI;
using UnityEngine;

namespace Common.Components.Audio
{
    public class UIObjectAudio : AudioComponent
    {
        [SerializeField]
        private UIObject uiObject;

        [SerializeField]
        private UIObjectActionType actionTypeType;

        private void Awake()
        {
            switch (actionTypeType)
            {
                case UIObjectActionType.UIObjectStart:
                    uiObject.UIObjectStart += OnUIObjectAction;
                    break;
                case UIObjectActionType.UIObjectStop:
                    uiObject.UIObjectStop += OnUIObjectAction;
                    break;
            }
        }

        private void OnDestroy()
        {
            switch (actionTypeType)
            {
                case UIObjectActionType.UIObjectStart:
                    uiObject.UIObjectStart -= OnUIObjectAction;
                    break;
                case UIObjectActionType.UIObjectStop:
                    uiObject.UIObjectStop -= OnUIObjectAction;
                    break;
            }
        }

        private void OnUIObjectAction()
        {
            PlaySound();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiObject == null) uiObject = GetComponent<UIObject>();
        }
#endif

        public enum UIObjectActionType
        {
            UIObjectStart,
            UIObjectStop
        }
    }
}