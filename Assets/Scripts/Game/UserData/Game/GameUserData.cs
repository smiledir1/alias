using System;
using System.Collections.Generic;
using Services.UserData;

namespace Game.UserData.Game
{
    [Serializable]
    public class GameUserData : UserDataObject
    {
        public override string DataName => nameof(GameUserData);

        public string wordsPacksConfigItemName;
        public int roundTimeSeconds;
        public bool isUnlimitedTimeForLastWord;
        public bool freeSkip;
        public List<TeamData> teams;
        public int currentRound;
        public List<int> playedWordsIndexes;
    }
}