using Common.Extensions;
using Game.Services.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Popups.Teams
{
    public class TeamPopupItem : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private Button _mainButton;

        private UnityAction<Team> _clickAction;
        private Team _team;
        
        public void Initialize(Team team, UnityAction<Team> clickAction)
        {
            _team = team;
            _clickAction = clickAction;
            _mainButton.SetClickListener(OnClick);
            _image.sprite = team.Icon;
            _name.text = team.Name;
        }

        private void OnClick()
        {
            _clickAction?.Invoke(_team);
        }
    }
}