#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class SceneOpenEditor
    {
        private const string IsAlwaysStartFromEntrySceneKey = "IsAlwaysStartFromEntryScene";
        private const string EntrySceneName = "EntryScene";
        private const string MetaSceneName = "MetaScene";
        private const string TestSceneName = "TestScene";

        private static bool _isAlwaysStartFromEntryScene;

        static SceneOpenEditor()
        {
            _isAlwaysStartFromEntryScene =
                EditorPrefs.GetBool(IsAlwaysStartFromEntrySceneKey, false);
            SetIsAlwaysStartFromEntryScene();
        }

        [MenuItem("Tools/Scenes/EntryScene %#&1", false, 0)]
        private static void OpenEntryScene()
        {
            OpenScene(EntrySceneName);
        }

        // [MenuItem("Tools/Scenes/MetaScene %#&2", false, 1)]
        // private static void OpenMainScene()
        // {
        //     OpenScene(_metaSceneName);
        // }
        
        [MenuItem("Tools/Scenes/TestScene %#&2", false, 1)]
        private static void OpenTestScene()
        {
            OpenScene(TestSceneName);
        }

        [MenuItem("Tools/Scenes/AlwaysStartFromEntryScene", false, 2)]
        private static void AlwaysStartFromEntryScene()
        {
            _isAlwaysStartFromEntryScene = !_isAlwaysStartFromEntryScene;
            EditorPrefs.SetBool(IsAlwaysStartFromEntrySceneKey, _isAlwaysStartFromEntryScene);
            SetIsAlwaysStartFromEntryScene();
        }

        [MenuItem("Tools/Scenes/Always Start From Entry Scene", true)]
        private static bool AlwaysStartFromEntrySceneValidate()
        {
            _isAlwaysStartFromEntryScene =
                EditorPrefs.GetBool(IsAlwaysStartFromEntrySceneKey, false);
            Menu.SetChecked("Tools/Scenes/AlwaysStartFromEntryScene",
                _isAlwaysStartFromEntryScene);
            return true;
        }

        private static void SetIsAlwaysStartFromEntryScene()
        {
            if (_isAlwaysStartFromEntryScene)
            {
                var scenePath = $"Assets/Scenes/{EntrySceneName}.unity";
                var myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (myWantedStartScene != null)
                    EditorSceneManager.playModeStartScene = myWantedStartScene;
                else
                    Debug.Log("Could not find Scene " + scenePath);
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        private static void OpenScene(string sceneName)
        {
            EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
        }
    }
}
#endif