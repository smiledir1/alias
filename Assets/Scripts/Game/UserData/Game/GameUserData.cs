using System;
using System.Collections.Generic;
using Services.UserData;

namespace Game.UserData.Game
{
    [Serializable]
    public class GameUserData : UserDataObject
    {
        public override string DataName => nameof(GameUserData);

        public string WordsPacksConfigItemName;
        public int RoundTimeSeconds;
        public bool IsUnlimitedTimeForLastWord;
        public bool FreeSkip;
        public List<TeamData> Teams;
        public int CurrentRound;
        public List<int> PlayedWordsIndexes;
    }
}