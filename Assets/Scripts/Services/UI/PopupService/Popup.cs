using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Services.UI.PopupService
{
    // TODO: another model for popup
    public class Popup : UIObject
    {
        [SerializeField]
        protected Button closeButton;

        protected override UniTask OnLoadAsync()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Close);
            }

            return base.OnLoadAsync();
        }

        #region Editor Helper

#if UNITY_EDITOR

        private void OnValidate()
        {
            CheckForCloseButton();
        }


        public bool CheckForCloseButton()
        {
            if (closeButton == null)
            {
                var buttons = gameObject.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                {
                    if (!button.gameObject.name.Contains("Close")) continue;
                    closeButton = button;
                    return true;
                }
            }

            return false;
        }

#endif

        #endregion /Editor Helper
    }

    public class Popup<T> : Popup where T : UIModel
    {
        protected T Model { get; private set; }

        internal override void Initialize(UIModel uiModel, int group)
        {
            base.Initialize(uiModel, group);
            Model = (T) uiModel;
            if (Model == null) Debug.LogError($"Wrong Model {typeof(T)} in {GetType()}");
        }
    }
}