using Services.Common;

namespace Services.UserData
{
    public interface IUserDataService : IService
    {
        T GetData<T>() where T : UserDataObject;
        void SaveUserData();
        void SaveUserData(UserDataObject userData);
        void SaveUserData<T>() where T : UserDataObject;
        void ClearData();
        void ClearData(UserDataObject userData);
        void ClearData<T>() where T : UserDataObject;
    }
}