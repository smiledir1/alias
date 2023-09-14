using UnityEditor;

namespace Common.CheckProject.Editor
{
    public class CheckProjectWindow : EditorWindow
    {
        private static CheckProjectWindow _window;

        //   [MenuItem("Tools/CheckProject/CheckProject Window")]
        private static void InitializeWindow()
        {
            _window = GetWindow<CheckProjectWindow>();
            _window.Show();

            Initialize();
        }

        private void OnGUI()
        {
            if (_window == null) InitializeWindow();
        }

        private static void Initialize()
        {
        }
    }
}