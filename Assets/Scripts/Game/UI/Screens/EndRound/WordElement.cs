using System;
using Game.UI.Screens.Round;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Screens.EndRound
{
    public class WordElement : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _wordLabel;

        [SerializeField]
        private Toggle _answeredToggle;

        public event Action<bool> AnsweredChange;

        public void Initialize(RoundWord roundWord)
        {
            _wordLabel.text = roundWord.Word;
            _answeredToggle.SetIsOnWithoutNotify(roundWord.IsRightAnswered);
            _answeredToggle.onValueChanged.RemoveAllListeners();
            _answeredToggle.onValueChanged.AddListener(OnToggleClick);
        }

        private void OnToggleClick(bool isOn)
        {
            AnsweredChange?.Invoke(isOn);
        }
    }
}