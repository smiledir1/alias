﻿using System.Collections.Generic;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Game.Services.WordsPacks;
using Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.Round
{
    public class RoundScreen : UIObject<RoundScreenModel>
    {
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

        private int _roundScore;
        private float _roundTimerSeconds;
        private WordsPack _wordsPack;
        private readonly HashSet<string> _playedPackWordsHash = new();
        private readonly List<string> _playedPackWords = new();
        private readonly List<RoundWord> _playedRoundWords = new();
        private string _currentWord;
        private int _generatedCount;
        private bool _waitLastWord;

        protected override UniTask OnOpenAsync()
        {
            _backButton.SetClickListener(OnBackButton);
            _rightAnswer.SetClickListener(OnRightAnswerButton);
            _wrongAnswer.SetClickListener(OnWrongAnswerButton);

            InitializeElements();

            _wordsPack = Model.WordsPacksConfigItem.WordsPack.Asset as WordsPack;

            StartGame().Forget();
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
            _playedPackWords.Clear();
            _playedRoundWords.Clear();
            _lastWord.SetActive(false);
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
            _roundScore--;
            _roundScoreLabel.text = _roundScore.ToString();
            var newRoundWord = new RoundWord(_currentWord, false);
            _playedRoundWords.Add(newRoundWord);
            CheckLastWord();
        }

        // По-хорошему под это отдельный объект завести,
        // но тут не критично
        private async UniTask StartGame()
        {
            ShowNewWord();
            await UniTask.Yield();
            while (_roundTimerSeconds > 0)
            {
                await UniTask.Yield();
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

        private void ShowNewWord()
        {
            _currentWord = GetNewWord();
            _currentWordLabel.text = _currentWord;
        }

        private string GetNewWord()
        {
            string word;
            do
            {
                var wordsPackWordsCount = _wordsPack.Words.Count;
                var wordPos = Random.Range(0, wordsPackWordsCount);
                word = _wordsPack.Words[wordPos];
            } while (_playedPackWordsHash.Contains(word));

            _playedPackWords.Add(word);
            _playedPackWordsHash.Add(word);
            _generatedCount++;

            if (_playedPackWords.Count <= _generatedCount)
            {
                _generatedCount = 0;
                _playedPackWords.Clear();
                _playedPackWordsHash.Clear();
            }

            return word;
        }

        private void FinishRound()
        {
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