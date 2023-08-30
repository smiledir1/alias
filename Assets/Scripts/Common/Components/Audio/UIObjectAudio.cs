using Services.Audio.Components;
using Services.UI;
using UnityEngine;

namespace Common.Components.Audio
{
    public class UIObjectAudio : AudioComponent
    {
        [SerializeField]
        private UIObject _uiObject;

        [SerializeField]
        private UIObjectActionType _actionTypeType;

        private void Awake()
        {
            switch (_actionTypeType)
            {
                case UIObjectActionType.UIObjectStart:
                    _uiObject.UIObjectStart += OnUIObjectAction;
                    break;
                case UIObjectActionType.UIObjectStop:
                    _uiObject.UIObjectStop += OnUIObjectAction;
                    break;
            }
        }

        private void OnDestroy()
        {
            switch (_actionTypeType)
            {
                case UIObjectActionType.UIObjectStart:
                    _uiObject.UIObjectStart -= OnUIObjectAction;
                    break;
                case UIObjectActionType.UIObjectStop:
                    _uiObject.UIObjectStop -= OnUIObjectAction;
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
            if (_uiObject == null)
            {
                _uiObject = GetComponent<UIObject>();
            }
        }
#endif

        public enum UIObjectActionType
        {
            UIObjectStart,
            UIObjectStop,
        }
    }
}