#if UNITY_EDITOR
using System.Collections.Generic;
using Common.UniTaskAnimations;
using Services.UI.PopupService;
using UnityEditor;
using UnityEngine;

namespace Services.UI.Editor
{
    public class UIHelperWindow : EditorWindow
    {
        private static UIHelperWindow _window;

        [SerializeField]
        [SerializeReference]
        private ITween _openSimpleTween;

        [SerializeField]
        [SerializeReference]
        private ITween _closeSimpleTween;

        private string _prefabPathFolder = "Assets/Prefabs";

        [MenuItem("Tools/UI Window")]
        private static void InitializeWindow()
        {
            _window = GetWindow<UIHelperWindow>();
            _window.Show();
        }

        private void OnGUI()
        {
            if (_window == null) InitializeWindow();
            var serializeWindowObject = new SerializedObject(this);

            _prefabPathFolder = EditorGUILayout.TextField("Path To Prefab Folder", _prefabPathFolder);

            if (GUILayout.Button("Set Tween as open animation")) MakeOpenAnimation();

            if (GUILayout.Button("Set Tween as close animation")) MakeCloseAnimation();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Check Close")) CheckCloseButton();

            if (GUILayout.Button("Check Empty Fields")) CheckEmptyFields();

            //TODO: check addressables on object
            //TODO: check animations (object and null)

            EditorGUILayout.EndHorizontal();

            serializeWindowObject.ApplyModifiedProperties();
        }

        private void MakeOpenAnimation()
        {
            var uiObjects = GetUIObjects<UIObject>();
            foreach (var uiObject in uiObjects)
            {
                var openAnimation = ITween.Clone(_openSimpleTween, uiObject.gameObject);
                uiObject.SetOpenAnimation(openAnimation);
                EditorUtility.SetDirty(uiObject);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Make Open Animation Complete");
        }

        private void MakeCloseAnimation()
        {
            var uiObjects = GetUIObjects<UIObject>();
            foreach (var uiObject in uiObjects)
            {
                var closeAnimation = ITween.Clone(_closeSimpleTween, uiObject.gameObject);
                uiObject.SetCloseAnimation(closeAnimation);
                EditorUtility.SetDirty(uiObject);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Make Close Animation Complete");
        }

        private void CheckCloseButton()
        {
            var uiObjects = GetUIObjects<Popup>();
            foreach (var uiObject in uiObjects)
            {
                if (!uiObject.CheckForCloseButton()) continue;
                EditorUtility.SetDirty(uiObject);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Check Close Complete");
        }

        private List<T> GetUIObjects<T>() where T : UIObject
        {
            var uiObjectsList = new List<T>();
            var assetsGuids = AssetDatabase.FindAssets("t:prefab", new[] {_prefabPathFolder});
            foreach (var guid in assetsGuids)
            {
                var prefab = AssetDatabase.GUIDToAssetPath(guid);
                var uiObject = AssetDatabase.LoadAssetAtPath<T>(prefab);
                if (uiObject == null) continue;
                uiObjectsList.Add(uiObject);
            }

            return uiObjectsList;
        }

        private void CheckEmptyFields()
        {
            var uiObjects = GetUIObjects<UIObject>();
            foreach (var uiObject in uiObjects)
            {
                CheckForEmptyFields(uiObject);
            }

            Debug.Log("Check For Empty Fields Complete");
        }

        private void CheckForEmptyFields(UIObject uiObject)
        {
            var serializedObject = new SerializedObject(uiObject);
            var propertiesIterator = serializedObject.GetIterator();
            for (var enterChildren = true; propertiesIterator.NextVisible(enterChildren); enterChildren = false)
            {
                var isEmpty = false;

                switch (propertiesIterator.propertyType)
                {
                    case SerializedPropertyType.ManagedReference:
                        if (propertiesIterator.managedReferenceId == -1) isEmpty = true;

                        break;
                    case SerializedPropertyType.ObjectReference:
                        if (propertiesIterator.objectReferenceInstanceIDValue == 0) isEmpty = true;

                        break;
                }

                if (isEmpty)
                {
                    Debug.Log($"UIObject: {uiObject.name} " +
                              $"Property: {propertiesIterator.propertyPath} " +
                              $"Type: {propertiesIterator.type} " +
                              $"Is Empty");
                }
            }
        }
    }
}
#endif