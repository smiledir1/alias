using System;
using Common.Extensions;
using Game.Services.WordsPacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Popups.ChoosePack
{
    public class ChoosePackItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _packName;

        [SerializeField]
        private TextMeshProUGUI _packDescription;

        [SerializeField]
        private TextMeshProUGUI _packExampleWords;

        [SerializeField]
        private TextMeshProUGUI _wordsCount;

        [SerializeField]
        private Button _chooseButton;

        public event Action<WordsPacksConfigItem> ChoosePack;

        private WordsPacksConfigItem _wordsPack;

        public void Initialize(WordsPacksConfigItem wordsPack)
        {
            _chooseButton.SetClickListener(OnChooseButton);
            _wordsPack = wordsPack;

            _packName.text = wordsPack.Name;
            _packDescription.text = wordsPack.Description;
            _packExampleWords.text = wordsPack.ExampleWords;
            _wordsCount.text = wordsPack.WordsCount;
        }

        private void OnChooseButton()
        {
            ChoosePack?.Invoke(_wordsPack);
        }
    }
}