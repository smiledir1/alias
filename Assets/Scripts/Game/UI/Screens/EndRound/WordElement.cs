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
        private TextMeshProUGUI wordLabel;

        [SerializeField]
        private Toggle answeredToggle;

        public event Action<bool> AnsweredChange;

        public void Initialize(RoundWord roundWord)
        {
            wordLabel.text = roundWord.Word;
            answeredToggle.SetIsOnWithoutNotify(roundWord.IsRightAnswered);
            answeredToggle.onValueChanged.RemoveAllListeners();
            answeredToggle.onValueChanged.AddListener(OnToggleClick);
        }

        private void OnToggleClick(bool isOn)
        {
            AnsweredChange?.Invoke(isOn);
        }
    }
}