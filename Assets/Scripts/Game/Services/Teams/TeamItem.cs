using System;
using UnityEngine;

namespace Game.Services.Teams
{
    [Serializable]
    public class TeamItem
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string nameLocalizationKey;

        [SerializeField]
        public Sprite icon;

        public string Id => id;
        public string NameLocalizationKey => nameLocalizationKey;
        public Sprite Icon => icon;
    }
}