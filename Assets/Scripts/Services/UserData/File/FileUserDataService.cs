using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Services.UserData.File
{
    // TODO: Strong Crypt
    // SaveBytes
    // Crypt Settings?
    public class FileUserDataService : BaseUserDataService
    {
        public FileUserDataService(List<UserDataObject> userDatCollection, bool isCrypt)
            : base(userDatCollection, isCrypt)
        {
        }

        protected override bool HasUserData(string key)
        {
            var path = Path.Combine(Application.persistentDataPath, key);
            return System.IO.File.Exists(path);
        }

        protected override string GetUserDataText(string key)
        {
            var path = Path.Combine(Application.persistentDataPath, key);
            return System.IO.File.ReadAllText(path);
        }

        protected override void ClearConcreteData(UserDataObject dataObject)
        {
            var dataName = dataObject.DataName;
            var path = Path.Combine(Application.persistentDataPath, dataName);
            System.IO.File.Delete(path);
        }

        protected override void SaveConcreteData(UserDataObject dataObject)
        {
            var path = Path.Combine(Application.persistentDataPath, dataObject.DataName);
            var userDataString = JsonConvert.SerializeObject(dataObject);
            if (IsCrypt) userDataString = Crypto(userDataString);

            System.IO.File.WriteAllText(path, userDataString);
        }
    }
}