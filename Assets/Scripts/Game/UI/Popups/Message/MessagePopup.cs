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
        private TextMeshProUGUI _header;
        
        [SerializeField]
        private TextMeshProUGUI _text;

        [Service]
        private static ILocalizationService _localizationService;
        
        protected override UniTask OnOpenAsync()
        {
            if (Model.Localize)
            {
                _header.text = _localizationService.GetText(Model.Title);
                _text.text = _localizationService.GetText(Model.Message);
            }
            else
            {
                _header.text = Model.Title;
                _text.text = Model.Message;
            }

            return base.OnOpenAsync();
        }
    }
}