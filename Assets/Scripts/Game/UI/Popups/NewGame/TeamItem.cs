using Game.Services.Teams;
using TMPro;
using UnityEngine;

namespace Game.UI.Popups.NewGame
{
    public class TeamItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;
        
        public void Initialize(Team team)
        {
            _name.text = team.Name;
        }
    }
}