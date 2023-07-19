using System;

namespace Game.UserData.Game
{
    [Serializable]
    public class TeamData
    {
        public string Id;
        public int Score;

        public TeamData(string id, int score)
        {
            Id = id;
            Score = score;
        }
    }
}