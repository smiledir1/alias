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
        private TextMeshProUGUI teamName;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Button removeButton;

        private UnityAction<TeamItem> _removeAction;

        public void Initialize(Team team, UnityAction<TeamItem> removeAction)
        {
            _removeAction = removeAction;
            removeButton.SetClickListener(OnRemoveButtonClick);
            teamName.text = team.Name;
            image.sprite = team.Icon;
        }

        private void OnRemoveButtonClick()
        {
            _removeAction?.Invoke(this);
        }
    }
}