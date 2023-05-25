using System;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Services.UI.PopupService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Popups.Confirm
{
    public class ConfirmPopup : Popup<ConfirmPopupModel>
    {
        [SerializeField]
        private TextMeshProUGUI _header;
        
        [SerializeField]
        private TextMeshProUGUI _text;
        
        [SerializeField]
        private Button _confirmButton;

        public event Action Confirm;
        
        protected override UniTask OnOpenAsync()
        {
            _header.text = Model.Title;
            _text.text = Model.Message;
            _confirmButton.SetListener(OnConfirmClick);
            _closeButton.SetListener(OnCloseClick);
            return base.OnOpenAsync();
        }
        
        private void OnConfirmClick()
        {
            Model.IsConfirm(true);
            Model.ConfirmCallback?.Invoke();
            Confirm?.Invoke();
            Close();
        }

        private void OnCloseClick()
        {
            Model.IsConfirm(false);
            Close();
        }
    }
}