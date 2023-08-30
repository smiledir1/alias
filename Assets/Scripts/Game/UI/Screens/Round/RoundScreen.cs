using System.Collections.Generic;
using System.Threading;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Game.Services.WordsPacks;
using Game.UserData.Game;
using Services.Helper;
using Services.Localization;
using Services.UI;
using Services.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Round
{
    public class RoundScreen : UIObject<RoundScreenModel>
    {
        private const string WordInPackKey = "in_pack_";
        [SerializeField]
        private TextMeshProUGUI _teamName;

        [SerializeField]
        private TextMeshProUGUI _roundScoreLabel;

        [SerializeField]
        private TextMeshProUGUI _roundTimerSecondsLabel;

        [SerializeField]
        private TextMeshProUGUI _currentWordLabel;

        [SerializeField]
        private GameObject _lastWord;

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private Button _rightAnswer;

        [SerializeField]
        private Button _wrongAnswer;
        
        [SerializeField]
        private Button _startButton;
        
        [SerializeField]
        private GameObject _gameFields;
        
        [SerializeField]
        private Button _stopButton;
        
        [SerializeField]
        private TextMeshProUGUI _wordInPack;

        private int _roundScore;
        private float _roundTimerSeconds;
        private WordsPack _wordsPack;
        private readonly HashSet<string> _playedPackWordsHash = new();
        private readonly List<int> _playedPackWordsIndexes = new();
        private readonly List<RoundWord> _playedRoundWords = new();
        private string _currentWord;
        private int _generatedCount;
        private bool _waitLastWord;
        private bool _isPause;
        private string _wordInPackLocalize;
        private CancellationTokenSource _gameTokenSource;

        [Service]
        private static IUserDataService _userData;
        
        [Service]
        private static ILocalizationService _localizationService;

        protected override UniTask OnOpenAsync()
        {
            _backButton.SetClickListener(OnBackButton);
            _rightAnswer.SetClickListener(OnRightAnswerButton);
            _wrongAnswer.SetClickListener(OnWrongAnswerButton);
            _startButton.SetClickListener(OnStartButton);
            _stopButton.SetClickListener(OnStopButton);
            
            InitializeElements();

            _wordsPack = Model.WordsPacksConfigItem.WordsPack.Asset as WordsPack;

            return base.OnOpenAsync();
        }

        private void OnBackButton()
        {
            Close();
            Model.GoBack();
        }

        private void InitializeElements()
        {
            _roundScore = 0;
            _waitLastWord = false;
            _teamName.text = Model.RoundTeam.Name;
            _roundScoreLabel.text = "0";
            _roundTimerSeconds = Model.RoundTimeSeconds;
            _roundTimerSecondsLabel.text = _roundTimerSeconds.ToString("F0");
            _generatedCount = 0;
            _playedPackWordsHash.Clear();
            _playedPackWordsIndexes.Clear();
            _playedRoundWords.Clear();
            _lastWord.SetActive(false);
            _startButton.gameObject.SetActive(true);
            _gameFields.SetActive(false);
            _wordInPackLocalize = _localizationService.GetText(WordInPackKey);
        }

        private void OnRightAnswerButton()
        {
            _roundScore++;
            _roundScoreLabel.text = _roundScore.ToString();
            var newRoundWord = new RoundWord(_currentWord, true);
            _playedRoundWords.Add(newRoundWord);
            CheckLastWord();
        }

        private void OnWrongAnswerButton()
        {
            if (!Model.FreeSkip) _roundScore--;
            _roundScoreLabel.text = _roundScore.ToString();
            var newRoundWord = new RoundWord(_currentWord, false);
            _playedRoundWords.Add(newRoundWord);
            CheckLastWord();
        }

        // По-хорошему под это отдельный объект завести,
        // но тут не критично
        private async UniTask StartGame(CancellationToken token)
        {
            _startButton.gameObject.SetActive(false);
            _gameFields.SetActive(true);
            LoadWords();
            ShowNewWord();
            await UniTask.Yield(token);
            while (_roundTimerSeconds > 0)
            {
                await UniTask.Yield(token);
                if (_isPause) continue;
                _roundTimerSeconds -= Time.deltaTime;
                _roundTimerSecondsLabel.text = _roundTimerSeconds.ToString("F0");
            }

            // end Game
            // last word
            if (Model.IsUnlimitedTimeForLastWord)
            {
                _waitLastWord = true;
                _lastWord.SetActive(true);
            }
            else
            {
                FinishRound();
            }
        }

        private void LoadWords()
        {
            var gameUserData = _userData.GetData<GameUserData>();
            if (gameUserData.PlayedWordsIndexes == null) return;
            foreach (var indexes in gameUserData.PlayedWordsIndexes)
            {
                _generatedCount++;
                _playedPackWordsHash.Add(_wordsPack.Words[indexes]);
                _playedPackWordsIndexes.Add(indexes);
            }
        }

        private void SaveWords()
        {
            var gameUserData = _userData.GetData<GameUserData>();
            var saveIndexes = new List<int>();
            foreach (var index in _playedPackWordsIndexes)
            {
                _generatedCount++;
                saveIndexes.Add(index);
            }

            gameUserData.PlayedWordsIndexes = saveIndexes;
        }

        private void ShowNewWord()
        {
            _currentWord = GetNewWord();
            _currentWordLabel.text = _currentWord;
            _wordInPack.text = string.Format(_wordInPackLocalize, _wordsPack.Words.Count - _generatedCount);
        }

        private string GetNewWord()
        {
            string word;
            int wordPos;
            var wordsPackWordsCount = _wordsPack.Words.Count;
            do
            {
                wordPos = Random.Range(0, wordsPackWordsCount);
                word = _wordsPack.Words[wordPos];
            } while (_playedPackWordsHash.Contains(word));
            
            _playedPackWordsIndexes.Add(wordPos);
            _playedPackWordsHash.Add(word);
            _generatedCount++;
            
            if (wordsPackWordsCount <= _generatedCount)
            {
                _generatedCount = 0;
                _playedPackWordsIndexes.Clear();
                _playedPackWordsHash.Clear();
            }
            
            return word;
        }
        
        

        private void FinishRound()
        {
            SaveWords();
            Model.EndRound(_playedRoundWords);
            Close();
        }

        private void CheckLastWord()
        {
            if (_waitLastWord)
            {
                FinishRound();
            }
            else
            {
                ShowNewWord();
            }
        }

        private void OnStartButton()
        {
            if (_isPause)
            {
                ResumeGame();
            }
            else
            {
                _gameTokenSource?.Cancel();
                _gameTokenSource = new CancellationTokenSource();
                StartGame(_gameTokenSource.Token).Forget();
            }
        }
        
        private void OnStopButton()
        {
            PauseGame();
        }

        private void PauseGame()
        {
            _startButton.gameObject.SetActive(true);
            _gameFields.SetActive(false);
            _isPause = true;
        }

        private void ResumeGame()
        {
            _startButton.gameObject.SetActive(false);
            _gameFields.SetActive(true);
            _isPause = false;
        }
    }

    public class RoundWord
    {
        public string Word { get; }
        public bool IsRightAnswered { get; }

        public RoundWord(string word, bool isRightAnswered)
        {
            Word = word;
            IsRightAnswered = isRightAnswered;
        }
    }
}