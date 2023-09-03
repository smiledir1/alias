using Services.Locator;
using Services.UserData;

namespace Common.Extensions
{
    public static class UserDataExtensions
    {
        private static FileUserDataService _fileUserDataService;
        
        public static void SaveData(this UserDataObject userData)
        {
            CheckService();
            _fileUserDataService.SaveUserData(userData);
        }
        
        public static void ClearData(this UserDataObject userData)
        {
            CheckService();
            _fileUserDataService.ClearData(userData);
        }

        private static void CheckService()
        {
            _fileUserDataService ??= ServiceLocator.GetService<FileUserDataService>();
        }
    }
}