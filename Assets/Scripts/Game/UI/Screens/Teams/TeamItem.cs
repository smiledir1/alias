using Game.Services.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Teams
{
    public class TeamItem : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI teamName;

        [SerializeField]
        private TextMeshProUGUI score;

        [SerializeField]
        private GameObject roundTeamMark;

        public void Initialize(Team team, bool isRoundTeam)
        {
            teamName.text = team.Name;
            score.text = team.Score.ToString();
            roundTeamMark.SetActive(isRoundTeam);
            icon.sprite = team.Icon;
        }
    }
}