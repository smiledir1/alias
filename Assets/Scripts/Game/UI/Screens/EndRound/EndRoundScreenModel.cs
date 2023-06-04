using System.Collections.Generic;
using Game.UI.Screens.Round;
using Services.UI;

namespace Game.UI.Screens.EndRound
{
    public record EndRoundScreenModel(
        List<RoundWord> RoundWords) : UIModel
    {
        public List<RoundWord> RoundWords { get; } = RoundWords;
    }
}