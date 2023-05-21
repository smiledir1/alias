using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services.Common;
using UnityEngine;
using File = System.IO.File;

namespace Services.UserData
{
    //TODO: Strong Crypt
    public class UserDataService : Service, IUserDataService
    {
        private const int _xorKey = 49;
        
        private readonly List<UserDataObject> _allData;
        private Dictionary<Type, UserDataObject> _userDatCollection;
        private readonly bool _isCrypt;

        public UserDataService(List<UserDataObject> userDatCollection, bool isCrypt)
        {
            _allData = userDatCollection;
            _isCrypt = isCrypt;
        }

        public T GetData<T>() where T : UserDataObject
        {
            if (_userDatCollection.TryGetValue(typeof(T), out var data))
            {
                return data as T;
            }
           
            Debug.LogError($"No Data: {typeof(T)}");
            return null;
        }

        public void SaveUserData()
        {
            foreach (var data in _userDatCollection.Values)
            {
                SaveConcreteData(data);
            }
        }

        public void SaveUserData(UserDataObject userData)
        {
            var type = userData.GetType();
            if (_userDatCollection.TryGetValue(type, out var data))
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
            if (_userDatCollection.TryGetValue(typeof(T), out var data))
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
            foreach (var data in _userDatCollection.Values)
            {
                ClearConcreteData(data);
            }
        }
        
        public void ClearData(UserDataObject userData)
        {
            var type = userData.GetType();
            if (_userDatCollection.TryGetValue(type, out var data))
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
            if (_userDatCollection.TryGetValue(typeof(T), out var data))
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

        private void LoadUserData()
        {
            _userDatCollection = new Dictionary<Type, UserDataObject>();
            foreach (var data in _allData)
            {
                var dataType = data.GetType();
                var dataName = data.DataName;
                var path = Path.Combine(Application.persistentDataPath, dataName);
                var hasUserData = File.Exists(path);
                if (hasUserData)
                {
                    try
                    {
                        var userDataText = File.ReadAllText(path);
                        if (_isCrypt)
                        {
                            userDataText = Crypto(userDataText);
                        }
                        
                        if (JsonConvert.DeserializeObject(userDataText, dataType) is UserDataObject loadedData)
                        {
                            _userDatCollection.Add(dataType, loadedData);
                        }
                        else
                        {
                            Debug.LogError($"Loaded Data null: {path} Type: {dataType}");
                        }
                    }
                    catch
                    {
                        Debug.LogError($"Wrong read: {path} Type: {dataType}");
                    }
                }
                else
                {
                    _userDatCollection.Add(dataType, data);
                }
            }
        }

        private void ClearConcreteData(UserDataObject dataObject)
        {
            var dataName = dataObject.DataName;
            var path = Path.Combine(Application.persistentDataPath, dataName);
            File.Delete(path);
        }

        private void SaveConcreteData(UserDataObject dataObject)
        {
            var path = Path.Combine(Application.persistentDataPath, dataObject.DataName);
            var userDataString = JsonConvert.SerializeObject(dataObject);
            if (_isCrypt)
            {
                userDataString = Crypto(userDataString);
            }
            File.WriteAllText(path, userDataString);
        }

        private string Crypto(string text)
        {
            var stringBuilder = new StringBuilder();
            for (var i = text.Length - 1; i >= 0; i--)
            {
                var t = text[i];
                var cryptChar = (char) (t ^ _xorKey);
                stringBuilder.Append(cryptChar);
            }
            return stringBuilder.ToString();
        }
    }
}