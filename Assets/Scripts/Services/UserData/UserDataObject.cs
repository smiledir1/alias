using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Services.UserData
{
    [Serializable]
    public abstract class UserDataObject
    {
        [JsonIgnore]
        public abstract string DataName { get; }
    }
}