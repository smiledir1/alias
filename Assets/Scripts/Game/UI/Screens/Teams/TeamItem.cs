using Game.Services.Teams;
using TMPro;
using UnityEngine;

namespace Game.UI.Screens.Teams
{
    public class TeamItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;
        
        [SerializeField]
        private TextMeshProUGUI _score;

        [SerializeField]
        private GameObject _roundTeamMark;
        
        public void Initialize(Team team, bool isRoundTeam)
        {
            _name.text = team.Name;
            _score.text = team.Score.ToString();
            _roundTeamMark.SetActive(isRoundTeam);
        }
    }
}