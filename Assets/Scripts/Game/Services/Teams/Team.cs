using UnityEngine;

namespace Game.Services.Teams
{
    public class Team
    {
        public Sprite Icon { get; }
        public string Name { get; }
        public string Id { get; }
        public int Score;

        internal Team(string name, string id, Sprite icon, int score)
        {
            Name = name;
            Id = id;
            Icon = icon;
            Score = score;
        }
    }
}