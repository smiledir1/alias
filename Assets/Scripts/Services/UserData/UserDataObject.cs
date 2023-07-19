using System;
using Newtonsoft.Json;

namespace Services.UserData
{
    [Serializable]
    public abstract class UserDataObject
    {
        [JsonIgnore]
        public abstract string DataName { get; }
    }
}