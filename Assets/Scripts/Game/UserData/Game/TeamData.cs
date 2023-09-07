using System;

namespace Game.UserData.Game
{
    [Serializable]
    public class TeamData
    {
        public string id;
        public int score;

        public TeamData(string id, int score)
        {
            this.id = id;
            this.score = score;
        }
    }
}