using System;
using System.Collections.Generic;
using Game.Services.Teams;
using Services.UserData;

namespace Game.UserData
{
    [Serializable]
    public class GameUserData : UserDataObject
    {
        public override string DataName => nameof(GameUserData);

        public string WordsPacksConfigItemName;
        public int RoundTimeSeconds;
        public bool IsUnlimitedTimeForLastWord;
        public List<Team> Teams;
        public int CurrentRound;
        
        //TODO: Add played words
    }
}