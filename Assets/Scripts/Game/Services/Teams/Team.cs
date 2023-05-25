using System;

namespace Game.Services.Teams
{
    [Serializable]
    public class Team
    {
        public string Name;
        public int Score;

        public Team(string name)
        {
            Name = name;
        }
    }
}