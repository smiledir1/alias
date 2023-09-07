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
        private Image image;

        [SerializeField]
        private TextMeshProUGUI teamName;

        [SerializeField]
        private Button mainButton;

        private UnityAction<Team> _clickAction;
        private Team _team;

        public void Initialize(Team team, UnityAction<Team> clickAction)
        {
            _team = team;
            _clickAction = clickAction;
            mainButton.SetClickListener(OnClick);
            image.sprite = team.Icon;
            teamName.text = team.Name;
        }

        private void OnClick()
        {
            _clickAction?.Invoke(_team);
        }
    }
}