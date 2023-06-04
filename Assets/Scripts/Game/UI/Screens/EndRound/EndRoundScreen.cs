using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.UI.Screens.Round;
using Services.UI;
using TMPro;
using UnityEngine;

namespace Game.UI.Screens.EndRound
{
    public class EndRoundScreen : UIObject<EndRoundScreenModel>
    {
        [SerializeField]
        private TextMeshProUGUI _scoreLabel;

        [SerializeField]
        private WordElement _wordElementTemplate;

        private int _currentScore;
        private readonly List<WordElement> _wordElements = new();

        protected override UniTask OnOpenAsync()
        {
            ClearAll();
            
            _currentScore = 0;
            foreach (var roundWord in Model.RoundWords)
            {
                _currentScore += roundWord.IsRightAnswered ? 1 : -1;
                CreateWordElement(roundWord);
            }

            _scoreLabel.text = _currentScore.ToString();
            return base.OnOpenAsync();
        }

        protected override UniTask OnCloseAsync()
        {
            ClearAll();
            return base.OnCloseAsync();
        }

        private void ClearAll()
        {
            foreach (var wordElement in _wordElements)
            {
                wordElement.AnsweredChange -= OnWordElementAnsweredChange;
                Destroy(wordElement.gameObject);
            }
            _wordElements.Clear();
            _wordElementTemplate.gameObject.SetActive(false);
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
        }
    }
}