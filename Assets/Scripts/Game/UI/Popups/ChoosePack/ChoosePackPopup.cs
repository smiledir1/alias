using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Services.WordsPacks;
using Services.Helper;
using Services.UI.PopupService;
using UnityEngine;

namespace Game.UI.Popups.ChoosePack
{
    public class ChoosePackPopup : Popup<ChoosePackPopupModel>
    {
        [SerializeField]
        private ChoosePackItem _choosePackItemTemplate;

        [Service]
        private IWordsPacksService _wordsPacksService;

        private readonly List<ChoosePackItem> _createdItems = new();

        protected override UniTask OnOpenAsync()
        {
            ClearItems();
            CreateItems(_wordsPacksService.WordsPacksConfig.WordsPacksItems);
            return base.OnOpenAsync();
        }

        protected override UniTask OnCloseAsync()
        {
            ClearItems();
            return base.OnCloseAsync();
        }

        private void CreateItems(List<WordsPacksConfigItem> configItems)
        {
            var parentTransform = _choosePackItemTemplate.transform.parent;
            foreach (var configItem in configItems)
            {
                var packItem = Instantiate(_choosePackItemTemplate, parentTransform);
                packItem.Initialize(configItem);
                packItem.ChoosePack += OnPackChoose;
                _createdItems.Add(packItem);
                packItem.gameObject.SetActive(true);
            }
        }

        private void ClearItems()
        {
            _choosePackItemTemplate.gameObject.SetActive(false);
            foreach (var item in _createdItems)
            {
                item.ChoosePack -= OnPackChoose;
                Destroy(item.gameObject);
            }

            _createdItems.Clear();
        }

        private void OnPackChoose(WordsPacksConfigItem configItem)
        {
            Model.SetChoosePack(configItem);
            Close();
        }
    }
}