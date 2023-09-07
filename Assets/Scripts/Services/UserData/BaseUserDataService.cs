using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services.Common;
using UnityEngine;

namespace Services.UserData
{
    public abstract class BaseUserDataService : Service, IUserDataService
    {
        private const int XorKey = 49;

        protected readonly List<UserDataObject> AllData;
        protected Dictionary<Type, UserDataObject> UserDatCollection;
        protected readonly bool IsCrypt;

        public BaseUserDataService(List<UserDataObject> userDatCollection, bool isCrypt)
        {
            AllData = userDatCollection;
            IsCrypt = isCrypt;
        }

        public T GetData<T>() where T : UserDataObject
        {
            if (UserDatCollection.TryGetValue(typeof(T), out var data)) return data as T;

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
                SaveConcreteData(data);
            else
                Debug.LogError($"No Data: {type}");
        }

        public void SaveUserData<T>() where T : UserDataObject
        {
            if (UserDatCollection.TryGetValue(typeof(T), out var data))
                SaveConcreteData(data);
            else
                Debug.LogError($"No Data: {typeof(T)}");
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
                ClearConcreteData(data);
            else
                Debug.LogError($"No Data: {type}");
        }

        public void ClearData<T>() where T : UserDataObject
        {
            if (UserDatCollection.TryGetValue(typeof(T), out var data))
                ClearConcreteData(data);
            else
                Debug.LogError($"No Data: {typeof(T)}");
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
                    try
                    {
                        var userDataText = GetUserDataText(dataName);
                        if (IsCrypt) userDataText = Crypto(userDataText);

                        if (JsonConvert.DeserializeObject(userDataText, dataType)
                            is UserDataObject loadedData)
                            UserDatCollection.Add(dataType, loadedData);
                        else
                            Debug.LogError($"Loaded Data null: {dataName} Type: {dataType}");
                    }
                    catch
                    {
                        Debug.LogError($"Wrong read: {dataName} Type: {dataType}");
                    }
                else
                    UserDatCollection.Add(dataType, data);
            }
        }

        protected abstract bool HasUserData(string key);

        protected abstract string GetUserDataText(string key);

        protected abstract void ClearConcreteData(UserDataObject dataObject);

        protected abstract void SaveConcreteData(UserDataObject dataObject);

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