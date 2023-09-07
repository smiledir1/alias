using System.Collections.Generic;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Game.Services.Teams;
using Game.Services.WordsPacks;
using Game.UI.Screens.EndRound;
using Game.UI.Screens.Round;
using Game.UI.Screens.Teams;
using Game.UserData.Game;
using Services.Advertisement;
using Services.Analytics;
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

        [Service]
        private static ITeamsService _teamsService;

        [Service]
        private static IAnalyticsService _analyticsService;

        [Service]
        private static IAdvertisementService _advertisementService;

        private readonly bool _isNewGame;
        private WordsPacksConfigItem _wordsPacksConfigItem;
        private int _roundTimeSeconds;
        private bool _isUnlimitedTimeForLastWord;
        private bool _freeSkip;
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
        public InGameState(
            WordsPacksConfigItem wordsPacksConfigItem,
            int roundTimeSeconds,
            bool isUnlimitedTimeForLastWord,
            bool freeSkip,
            List<Team> teams)
        {
            _isNewGame = true;
            _wordsPacksConfigItem = wordsPacksConfigItem;
            _roundTimeSeconds = roundTimeSeconds;
            _isUnlimitedTimeForLastWord = isUnlimitedTimeForLastWord;
            _freeSkip = freeSkip;
            _teams = teams;
        }

        protected override async UniTask OnEnterState()
        {
            var currentRound = 1;
            if (!_isNewGame)
            {
                currentRound = LoadGame();
            }
            else
            {
                var gameUserData = _userData.GetData<GameUserData>();
                gameUserData.PlayedWordsIndexes = new List<int>();
            }

            if (_wordsPacksConfigItem.WordsPack.Asset == null) await _wordsPacksConfigItem.WordsPack.LoadAssetAsync();

            StartRound(currentRound).Forget();
            SendGameEvent(
                currentRound.ToString(),
                _wordsPacksConfigItem.Name,
                _roundTimeSeconds.ToString(),
                _isUnlimitedTimeForLastWord.ToString(),
                _freeSkip.ToString(),
                _teams.Count.ToString());
            await base.OnEnterState();
        }

        private async UniTask StartRound(int round)
        {
            SendStartRoundEvent(round);
            SaveGame(round);
            var isStartRound = await StartRoundScreen(round);
            if (!isStartRound)
            {
                GotoMetaState();
                return;
            }

            var playedWords = await PlayGameRound(round);
            if (playedWords == null)
            {
                StartRound(round).Forget();
                return;
            }

            await EndGameRound(round, playedWords);


            SendEndRoundEvent(round, playedWords.Count);

            var newRound = round + 1;
            StartRound(newRound).Forget();
            _advertisementService.ShowInterstitialAd().Forget();
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
                _isUnlimitedTimeForLastWord,
                _freeSkip);
            _screenService.ShowAsync<RoundScreen>(gameScreenModel).Forget();
            return await gameScreenModel.WaitForRound();
        }

        private async UniTask EndGameRound(int round, List<RoundWord> roundWords)
        {
            var currentRound = round - 1;
            var teamsCount = _teams.Count;
            var roundTeamPos = currentRound % teamsCount;
            var roundTeam = _teams[roundTeamPos];
            var endGameScreenModel = new EndRoundScreenModel(roundTeam, roundWords, _freeSkip);
            _screenService.ShowAsync<EndRoundScreen>(endGameScreenModel).Forget();
            await endGameScreenModel.WaitForClose();
        }

        private void SaveGame(int round)
        {
            var gameUserData = _userData.GetData<GameUserData>();
            gameUserData.WordsPacksConfigItemName = _wordsPacksConfigItem.Name;
            gameUserData.RoundTimeSeconds = _roundTimeSeconds;
            gameUserData.IsUnlimitedTimeForLastWord = _isUnlimitedTimeForLastWord;
            gameUserData.FreeSkip = _freeSkip;
            gameUserData.Teams = _teamsService.CreateDataFromTeams(_teams);
            gameUserData.CurrentRound = round;
            _userData.SaveUserData<GameUserData>();
        }

        private int LoadGame()
        {
            var gameUserData = _userData.GetData<GameUserData>();
            _wordsPacksConfigItem = _wordsPacksService.WordsPacksConfig.WordsPacksItems.Find(
                x => x.Name == gameUserData.WordsPacksConfigItemName);
            if (_wordsPacksConfigItem == null) return 0;
            _roundTimeSeconds = gameUserData.RoundTimeSeconds;
            _isUnlimitedTimeForLastWord = gameUserData.IsUnlimitedTimeForLastWord;
            _freeSkip = gameUserData.FreeSkip;
            _teams = _teamsService.CreateTeamsFromData(gameUserData.Teams);
            return gameUserData.CurrentRound;
        }

        private void SendGameEvent(
            string round,
            string packName,
            string roundTime,
            string lastWord,
            string freeSkip,
            string teamsCount)
        {
            var parameters = new List<Parameter>
            {
                new("round", round),
                new("pack_name", packName),
                new("round_time", roundTime),
                new("last_word", lastWord),
                new("free_skip", freeSkip),
                new("teams_count", teamsCount)
            };
            _analyticsService.SendEvent("start_round", parameters);
        }

        private void SendStartRoundEvent(int round)
        {
            var parameters = new List<Parameter>
            {
                new("round", round.ToString())
            };
            _analyticsService.SendEvent("start_round", parameters);
        }

        private void SendEndRoundEvent(int round, int playerWordsCount)
        {
            var parameters = new List<Parameter>
            {
                new("round", round.ToString()),
                new("player_words_count", playerWordsCount.ToString())
            };
            _analyticsService.SendEvent("start_round", parameters);
        }
    }
}