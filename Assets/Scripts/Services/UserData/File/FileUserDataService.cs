using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services.Common;
using UnityEngine;

namespace Services.UserData.File
{
    // TODO: Strong Crypt
    // SaveBytes
    // Crypt Settings?
    public class FileUserDataService : Service, IUserDataService
    {
        private const int XorKey = 49;

        protected readonly List<UserDataObject> AllData;
        protected Dictionary<Type, UserDataObject> UserDatCollection;
        protected readonly bool IsCrypt;

        public FileUserDataService(List<UserDataObject> userDatCollection, bool isCrypt)
        {
            AllData = userDatCollection;
            IsCrypt = isCrypt;
        }

        public T GetData<T>() where T : UserDataObject
        {
            if (UserDatCollection.TryGetValue(typeof(T), out var data))
            {
                return data as T;
            }

            Debug.LogError($"No Data: {typeof(T)}");
            return null;
        }

        public void SaveUserData()
        {
            foreach (var data in UserDatCollection.Values)
            {
                SaveConcreteData(data);
            }
        }

        public void SaveUserData(UserDataObject userData)
        {
            var type = userData.GetType();
            if (UserDatCollection.TryGetValue(type, out var data))
            {
                SaveConcreteData(data);
            }
            else
            {
                Debug.LogError($"No Data: {type}");
            }
        }

        public void SaveUserData<T>() where T : UserDataObject
        {
            if (UserDatCollection.TryGetValue(typeof(T), out var data))
            {
                SaveConcreteData(data);
            }
            else
            {
                Debug.LogError($"No Data: {typeof(T)}");
            }
        }

        public void ClearData()
        {
            foreach (var data in UserDatCollection.Values)
            {
                ClearConcreteData(data);
            }
        }

        public void ClearData(UserDataObject userData)
        {
            var type = userData.GetType();
            if (UserDatCollection.TryGetValue(type, out var data))
            {
                ClearConcreteData(data);
            }
            else
            {
                Debug.LogError($"No Data: {type}");
            }
        }

        public void ClearData<T>() where T : UserDataObject
        {
            if (UserDatCollection.TryGetValue(typeof(T), out var data))
            {
                ClearConcreteData(data);
            }
            else
            {
                Debug.LogError($"No Data: {typeof(T)}");
            }
        }

        protected override UniTask OnInitialize()
        {
            LoadUserData();
            return base.OnInitialize();
        }

        protected virtual void LoadUserData()
        {
            UserDatCollection = new Dictionary<Type, UserDataObject>();
            foreach (var data in AllData)
            {
                var dataType = data.GetType();
                var dataName = data.DataName;
                var hasUserData = HasUserData(dataName);
                if (hasUserData)
                {
                    try
                    {
                        var userDataText = GetUserDataText(dataName);
                        if (IsCrypt)
                        {
                            userDataText = Crypto(userDataText);
                        }

                        if (JsonConvert.DeserializeObject(userDataText, dataType) 
                            is UserDataObject loadedData)
                        {
                            UserDatCollection.Add(dataType, loadedData);
                        }
                        else
                        {
                            Debug.LogError($"Loaded Data null: {dataName} Type: {dataType}");
                        }
                    }
                    catch
                    {
                        Debug.LogError($"Wrong read: {dataName} Type: {dataType}");
                    }
                }
                else
                {
                    UserDatCollection.Add(dataType, data);
                }
            }
        }

        protected virtual bool HasUserData(string key)
        {
            var path = Path.Combine(Application.persistentDataPath, key);
            return System.IO.File.Exists(path);
        }
        
        protected virtual string GetUserDataText(string key)
        {
            var path = Path.Combine(Application.persistentDataPath, key);
            return System.IO.File.ReadAllText(path);
        }

        protected virtual void ClearConcreteData(UserDataObject dataObject)
        {
            var dataName = dataObject.DataName;
            var path = Path.Combine(Application.persistentDataPath, dataName);
            System.IO.File.Delete(path);
        }

        protected virtual void SaveConcreteData(UserDataObject dataObject)
        {
            var path = Path.Combine(Application.persistentDataPath, dataObject.DataName);
            var userDataString = JsonConvert.SerializeObject(dataObject);
            if (IsCrypt)
            {
                userDataString = Crypto(userDataString);
            }

            System.IO.File.WriteAllText(path, userDataString);
        }

        protected string Crypto(string text)
        {
            var stringBuilder = new StringBuilder();
            for (var i = text.Length - 1; i >= 0; i--)
            {
                var t = text[i];
                var cryptChar = (char) (t ^ XorKey);
                stringBuilder.Append(cryptChar);
            }

            return stringBuilder.ToString();
        }
    }
}