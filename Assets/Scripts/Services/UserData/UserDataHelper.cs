#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Services.UserData
{
    public static class UserDataHelper
    {
        [UnityEditor.MenuItem("Tools/UserData/Log File Path")]
        public static void LogFilePath()
        {
            var path = Path.Combine(Application.persistentDataPath);
            Debug.Log(path);
        }

        [UnityEditor.MenuItem("Tools/UserData/Open")]
        public static void OpenUserData()
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }
}
#endif