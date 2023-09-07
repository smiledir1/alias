using Cysharp.Threading.Tasks;
using Services.Helper;
using Services.Localization;
using Services.UI.PopupService;
using TMPro;
using UnityEngine;

namespace Game.UI.Popups.Message
{
    public class MessagePopup : Popup<MessagePopupModel>
    {
        [SerializeField]
        private TextMeshProUGUI header;

        [SerializeField]
        private TextMeshProUGUI text;

        [Service]
        private static ILocalizationService _localizationService;

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

            return base.OnOpenAsync();
        }
    }
}