using Services.Locator;
using Services.UserData;

namespace Common.Extensions
{
    public static class UserDataExtensions
    {
        private static UserDataService _userDataService;
        
        public static void SaveData(this UserDataObject userData)
        {
            CheckService();
            _userDataService.SaveUserData(userData);
        }
        
        public static void ClearData(this UserDataObject userData)
        {
            CheckService();
            _userDataService.ClearData(userData);
        }

        private static void CheckService()
        {
            _userDataService ??= ServiceLocator.GetService<UserDataService>();
        }
    }
}