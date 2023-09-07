using System;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Services.Helper;
using Services.Localization;
using Services.UI.PopupService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Popups.Confirm
{
    public class ConfirmPopup : Popup<ConfirmPopupModel>
    {
        [SerializeField]
        private TextMeshProUGUI header;

        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        private Button confirmButton;

        [Service]
        private static ILocalizationService _localizationService;

        public event Action Confirm;

        protected override UniTask OnOpenAsync()
        {
            if (Model.Localize)
            {
                header.text = _localizationService.GetText(Model.Title);
                text.text = _localizationService.GetText(Model.Message);
            }
            else
            {
                header.text = Model.Title;
                text.text = Model.Message;
            }

            confirmButton.SetClickListener(OnConfirmClick);
            _closeButton.SetClickListener(OnCloseClick);
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