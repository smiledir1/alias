using System.Collections.Generic;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Game.UI.Screens.Round;
using Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.EndRound
{
    public class EndRoundScreen : UIObject<EndRoundScreenModel>
    {
        [SerializeField]
        private TextMeshProUGUI _scoreLabel;

        [SerializeField]
        private WordElement _wordElementTemplate;

        [SerializeField]
        private Button _closeButton;

        private int _currentScore;
        private readonly List<WordElement> _wordElements = new();

        protected override UniTask OnOpenAsync()
        {
            _closeButton.SetClickListener(Close);

            ClearAll();

            _currentScore = 0;
            foreach (var roundWord in Model.RoundWords)
            {
                _currentScore += CountScore(roundWord.IsRightAnswered);
                CreateWordElement(roundWord);
            }

            _scoreLabel.text = _currentScore.ToString();
            return base.OnOpenAsync();
        }

        protected override UniTask OnCloseAsync()
        {
            Model.CurrentTeam.Score += _currentScore;
            ClearAll();
            return base.OnCloseAsync();
        }

        private void ClearAll()
        {
            _wordElementTemplate.gameObject.SetActive(false);
            foreach (var wordElement in _wordElements)
            {
                wordElement.AnsweredChange -= OnWordElementAnsweredChange;
                Destroy(wordElement.gameObject);
            }

            _wordElements.Clear();
        }

        private void CreateWordElement(RoundWord roundWord)
        {
            var wordElementParent = _wordElementTemplate.transform.parent;
            var wordElement = Instantiate(_wordElementTemplate, wordElementParent);
            wordElement.Initialize(roundWord);
            wordElement.AnsweredChange += OnWordElementAnsweredChange;
            wordElement.gameObject.SetActive(true);
            _wordElements.Add(wordElement);
        }

        private void OnWordElementAnsweredChange(bool isOn)
        {
            _currentScore += isOn ? 1 : -1;
            _scoreLabel.text = _currentScore.ToString();
        }

        private int CountScore(bool isRightAnswered) =>
            isRightAnswered
                ? 1
                : Model.FreeSkip
                    ? 0
                    : -1;
    }
}