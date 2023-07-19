using Common.Extensions;
using Game.Services.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Popups.NewGame
{
    public class TeamItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;
        
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Button _removeButton;

        private UnityAction<TeamItem> _removeAction;

        public void Initialize(Team team, UnityAction<TeamItem> removeAction)
        {
            _removeAction = removeAction;
            _removeButton.SetClickListener(OnRemoveButtonClick);
            _name.text = team.Name;
            _image.sprite = team.Icon;
        }

        private void OnRemoveButtonClick()
        {
            _removeAction?.Invoke(this);
        }
    }
}