using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Services.UserData
{
    public class WebGLUserDataService : UserDataService
    {
        [DllImport("__Internal")]
        private static extern int HasKeyInLocalStorage(string key);
        
        [DllImport("__Internal")]
        private static extern string LoadFromLocalStorage(string key);
        
        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);
        
        [DllImport("__Internal")]
        private static extern void RemoveFromLocalStorage(string key);

        public WebGLUserDataService(List<UserDataObject> userDatCollection, bool isCrypt) 
            : base(userDatCollection, isCrypt)
        {
        }

        protected override void SaveConcreteData(UserDataObject dataObject)
        {
            var userDataString = JsonConvert.SerializeObject(dataObject);
            if (_isCrypt)
            {
                userDataString = Crypto(userDataString);
            }
            SaveToLocalStorage(dataObject.DataName, userDataString);
        }

        protected override void ClearConcreteData(UserDataObject dataObject)
        {
            RemoveFromLocalStorage(dataObject.DataName);
        }

        protected override bool HasUserData(string key)
        {
            return HasKeyInLocalStorage(key) == 1;
        }
        
        protected override string GetUserDataText(string key)
        {
            return LoadFromLocalStorage(key);
        }
    }
}