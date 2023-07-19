using System;
using UnityEngine;

namespace Game.Services.Teams
{
    [Serializable]
    public class TeamItem
    {
        [SerializeField]
        private string _id;
            
        [SerializeField]
        private string _nameLocalizationKey;

        [SerializeField]
        private Sprite _icon;

        public string Id => _id;
        public string NameLocalizationKey => _nameLocalizationKey;
        public Sprite Icon => _icon;
    }
}