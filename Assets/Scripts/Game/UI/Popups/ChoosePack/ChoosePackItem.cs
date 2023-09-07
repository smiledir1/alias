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

            packName.text = wordsPack.name;
            packDescription.text = wordsPack.description;
            packExampleWords.text = wordsPack.exampleWords;
            wordsCount.text = wordsPack.wordsCount;
        }

        private void OnChooseButton()
        {
            ChoosePack?.Invoke(_wordsPack);
        }
    }
}