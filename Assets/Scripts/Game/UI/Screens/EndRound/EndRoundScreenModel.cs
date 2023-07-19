﻿using System.Collections.Generic;
using Game.Services.Teams;
using Game.UI.Screens.Round;
using Services.UI;

namespace Game.UI.Screens.EndRound
{
    public record EndRoundScreenModel(
        Team CurrentTeam,
        List<RoundWord> RoundWords,
        bool FreeSkip) : UIModel
    {
        public List<RoundWord> RoundWords { get; } = RoundWords;
        public Team CurrentTeam { get; } = CurrentTeam;
        public bool FreeSkip { get; } = FreeSkip;
    }
}