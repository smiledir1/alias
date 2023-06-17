using System.Collections.Generic;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.UI.Screens.EndRound;
using Game.UI.Screens.Round;
using Game.UI.Screens.StartRound;
using Game.UI.Screens.Teams;
using Game.UserData;
using Services.Helper;
using Services.UI.ScreenService;
using Services.UserData;

namespace Game.States
{
    public class InGameState : GameState
    {
        [Service]
        private static IScreenService _screenService;
        
        [Service]
        private static IUserDataService _userData;
        
        [Service]
        private static IWordsPacksService _wordsPacksService;

        private readonly bool _isNewGame;
        private WordsPacksConfigItem _wordsPacksConfigItem;
        private int _roundTimeSeconds;
        private bool _isUnlimitedTimeForLastWord;
        private List<Team> _teams;
        
        /// <summary>
        /// Load Last Game
        /// </summary>
        public InGameState()
        {
            _isNewGame = false;
        }
        
        /// <summary>
        /// Create new game
        /// </summary>
        /// <param name="wordsPacksConfigItem"></param>
        /// <param name="roundTimeSeconds"></param>
        /// <param name="isUnlimitedTimeForLastWord"></param>
        /// <param name="teams"></param>
        public InGameState(
            WordsPacksConfigItem wordsPacksConfigItem,
            int roundTimeSeconds,
            bool isUnlimitedTimeForLastWord,
            List<Team> teams)
        {
            _isNewGame = true;
            _wordsPacksConfigItem = wordsPacksConfigItem;
            _roundTimeSeconds = roundTimeSeconds;
            _isUnlimitedTimeForLastWord = isUnlimitedTimeForLastWord;
            _teams = teams;
        }

        protected override async UniTask OnEnterState()
        {
            var currentRound = 1;
            if (!_isNewGame)
            {
                currentRound = LoadGame();
            }
            
            if (_wordsPacksConfigItem.WordsPack.Asset == null)
            {
                await _wordsPacksConfigItem.WordsPack.LoadAssetAsync();
            }
            
            StartRound(currentRound).Forget();

            await base.OnEnterState();
        }

        private async UniTask StartRound(int round)
        {
            SaveGame(round);
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
            var currentRound = round - 1;
            var teamsCount = _teams.Count;
            var roundTeamPos = currentRound % teamsCount;
            var roundTeam = _teams[roundTeamPos];
            var endGameScreenModel = new EndRoundScreenModel(roundTeam, roundWords);
            _screenService.ShowAsync<EndRoundScreen>(endGameScreenModel).Forget();
            await endGameScreenModel.WaitForClose();
            var newRound = round + 1;
            StartRound(newRound).Forget();
        }

        private void SaveGame(int round)
        {
            var gameUserData = _userData.GetData<GameUserData>();
            gameUserData.WordsPacksConfigItemName = _wordsPacksConfigItem.Name;
            gameUserData.RoundTimeSeconds = _roundTimeSeconds;
            gameUserData.IsUnlimitedTimeForLastWord = _isUnlimitedTimeForLastWord;
            gameUserData.Teams = _teams;
            gameUserData.CurrentRound = round;
            _userData.SaveUserData<GameUserData>();
        }

        private int LoadGame()
        {
            var gameUserData = _userData.GetData<GameUserData>();
            _wordsPacksConfigItem =_wordsPacksService.WordsPacksConfig.WordsPacksItems.Find(
                x => x.Name == gameUserData.WordsPacksConfigItemName);
            if (_wordsPacksConfigItem == null) return 0;
            _roundTimeSeconds = gameUserData.RoundTimeSeconds;
            _isUnlimitedTimeForLastWord = gameUserData.IsUnlimitedTimeForLastWord;
            _teams = gameUserData.Teams;
            return gameUserData.CurrentRound;
        }
    }
}