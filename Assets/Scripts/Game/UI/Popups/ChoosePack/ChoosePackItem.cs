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
        private TextMeshProUGUI packName;

        [SerializeField]
        private TextMeshProUGUI packDescription;

        [SerializeField]
        private TextMeshProUGUI packExampleWords;

        [SerializeField]
        private TextMeshProUGUI wordsCount;

        [SerializeField]
        private Button chooseButton;

        public event Action<WordsPacksConfigItem> ChoosePack;

        private WordsPacksConfigItem _wordsPack;

        public void Initialize(WordsPacksConfigItem wordsPack)
        {
            chooseButton.SetClickListener(OnChooseButton);
            _wordsPack = wordsPack;

            packName.text = wordsPack.Name;
            packDescription.text = wordsPack.Description;
            packExampleWords.text = wordsPack.ExampleWords;
            wordsCount.text = wordsPack.WordsCount;
        }

        private void OnChooseButton()
        {
            ChoosePack?.Invoke(_wordsPack);
        }
    }
}