using Services.Common;

namespace Services.UserData
{
    public interface IUserDataService : IService
    {
        T GetData<T>() where T : UserDataObject;
        void SaveUserData();
        void SaveUserData<T>() where T : UserDataObject;
        void ClearData();
        void ClearData<T>() where T : UserDataObject;
    }
}