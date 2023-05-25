using Cysharp.Threading.Tasks;
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

        protected override UniTask OnOpenAsync()
        {
            _header.text = Model.Title;
            _text.text = Model.Message;
            return base.OnOpenAsync();
        }
    }
}