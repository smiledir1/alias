using System.Collections.Generic;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.UI.Screens.EndRound;
using Game.UI.Screens.Game;
using Game.UI.Screens.StartGame;
using Game.UI.Screens.Teams;
using Services.Helper;
using Services.UI.ScreenService;

namespace Game.States
{
    public class InGameState : GameState
    {
        [Service]
        private static IScreenService _screenService;

        private readonly WordsPacksConfigItem _wordsPacksConfigItem;
        private readonly int _roundTimeSeconds;
        private readonly bool _isUnlimitedTimeForLastWord;
        private readonly List<Team> _teams;

        public InGameState(
            WordsPacksConfigItem wordsPacksConfigItem,
            int roundTimeSeconds,
            bool isUnlimitedTimeForLastWord,
            List<Team> teams)
        {
            _wordsPacksConfigItem = wordsPacksConfigItem;
            _roundTimeSeconds = roundTimeSeconds;
            _isUnlimitedTimeForLastWord = isUnlimitedTimeForLastWord;
            _teams = teams;
        }

        protected override UniTask OnEnterState()
        {
            StartRound(0).Forget();
            return base.OnEnterState();
        }

        private async UniTask StartRound(int round)
        {
            var isStartRound = await StartRoundScreen(round);
            if (!isStartRound)
            {
                GotoMetaState();
                return;
            }

            isStartRound = await ConfirmStartRoundScreen();
            if (!isStartRound)
            {
                StartRound(round).Forget();
                return;
            }

            var isGameSkip = !await PlayGameRound();
            if (isGameSkip)
            {
                StartRound(round).Forget();
                return;
            }

            await EndGameRound(round);
        }

        private async UniTask<bool> StartRoundScreen(int round)
        {
            var teamsScreenModel = new TeamsScreenModel(
                _wordsPacksConfigItem,
                _roundTimeSeconds,
                _isUnlimitedTimeForLastWord,
                _teams,
                round);
            _screenService.ShowAsync<TeamsScreen>(teamsScreenModel).Forget();
            return await teamsScreenModel.WaitForStart();
        }

        private void GotoMetaState()
        {
            var metaState = new MetaGameState();
            metaState.GoToState().Forget();
        }

        private async UniTask<bool> ConfirmStartRoundScreen()
        {
            var startGameScreenModel = new StartRoundScreenModel();
            _screenService.ShowAsync<StartRoundScreen>(startGameScreenModel).Forget();
            return await startGameScreenModel.WaitForStart();
        }

        private async UniTask<bool> PlayGameRound()
        {
            var gameScreenModel = new RoundScreenModel();
            _screenService.ShowAsync<RoundScreen>(gameScreenModel).Forget();
            return await gameScreenModel.WaitForRound();
        }

        private async UniTask EndGameRound(int round)
        {
            var endGameScreenModel = new EndRoundScreenModel();
            _screenService.ShowAsync<EndRoundScreen>(endGameScreenModel).Forget();
            await endGameScreenModel.WaitForClose();
            var newRound = round + 1;
            StartRound(newRound).Forget();
        }
    }
}