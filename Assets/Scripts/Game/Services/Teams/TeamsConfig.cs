using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.Teams
{
    [CreateAssetMenu(fileName = nameof(TeamsConfig), menuName = "Services/Teams/TeamsConfig")]
    public class TeamsConfig : ScriptableObject
    {
        public List<TeamItem> teams;
    }
}