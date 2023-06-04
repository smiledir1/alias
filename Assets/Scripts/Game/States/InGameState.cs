using System.Collections.Generic;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.UI.Screens.EndRound;
using Game.UI.Screens.Round;
using Game.UI.Screens.StartRound;
using Game.UI.Screens.Teams;
using Services.Helper;
using Services.UI.ScreenService;

namespace Game.States
{
    public class InGameState : GameState
    {
        [Service]
        private static IScreenService _screenService;

        private readonly bool _isNewGame;
        private readonly WordsPacksConfigItem _wordsPacksConfigItem;
        private readonly int _roundTimeSeconds;
        private readonly bool _isUnlimitedTimeForLastWord;
        private readonly List<Team> _teams;

        public InGameState(
            bool isNewGame,
            WordsPacksConfigItem wordsPacksConfigItem,
            int roundTimeSeconds,
            bool isUnlimitedTimeForLastWord,
            List<Team> teams)
        {
            _isNewGame = isNewGame;
            _wordsPacksConfigItem = wordsPacksConfigItem;
            _roundTimeSeconds = roundTimeSeconds;
            _isUnlimitedTimeForLastWord = isUnlimitedTimeForLastWord;
            _teams = teams;
        }

        protected override async UniTask OnEnterState()
        {
            await _wordsPacksConfigItem.WordsPack.LoadSceneAsync();
           
            if (_isNewGame)
            {
                StartRound(1).Forget();
            }
            else
            {
                // load game
            }

            await base.OnEnterState();
        }

        private async UniTask StartRound(int round)
        {
            var isStartRound = await StartRoundScreen(round);
            if (!isStartRound)
            {
                GotoMetaState();
                return;
            }

            isStartRound = await ConfirmStartRoundScreen(round);
            if (!isStartRound)
            {
                StartRound(round).Forget();
                return;
            }

            var playedWords = await PlayGameRound(round);
            if (playedWords == null)
            {
                StartRound(round).Forget();
                return;
            }

            await EndGameRound(round, playedWords);
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

        private async UniTask<bool> ConfirmStartRoundScreen(int round)
        {
            var currentRound = round - 1;
            var teamsCount = _teams.Count;
            var roundTeamPos = currentRound % teamsCount;
            var roundTeam = _teams[roundTeamPos];
            var startGameScreenModel = new StartRoundScreenModel(roundTeam);
            _screenService.ShowAsync<StartRoundScreen>(startGameScreenModel).Forget();
            return await startGameScreenModel.WaitForStart();
        }

        private async UniTask<List<RoundWord>> PlayGameRound(int round)
        {
            var currentRound = round - 1;
            var teamsCount = _teams.Count;
            var roundTeamPos = currentRound % teamsCount;
            var roundTeam = _teams[roundTeamPos];
            var gameScreenModel = new RoundScreenModel(
                roundTeam,
                _roundTimeSeconds,
                _wordsPacksConfigItem,
                _isUnlimitedTimeForLastWord);
            _screenService.ShowAsync<RoundScreen>(gameScreenModel).Forget();
            return await gameScreenModel.WaitForRound();
        }

        private async UniTask EndGameRound(int round, List<RoundWord> roundWords)
        {
            var endGameScreenModel = new EndRoundScreenModel(roundWords);
            _screenService.ShowAsync<EndRoundScreen>(endGameScreenModel).Forget();
            await endGameScreenModel.WaitForClose();
            var newRound = round + 1;
            StartRound(newRound).Forget();
        }
    }
}