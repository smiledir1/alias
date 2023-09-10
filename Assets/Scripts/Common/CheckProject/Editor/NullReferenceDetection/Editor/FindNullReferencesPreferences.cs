using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.CheckProject.Editor.NullReferenceDetection.Editor
{
    public class FindNullReferencesPreferences : MonoBehaviour
    {
        private const float CellMargin = 6;

        private static List<Tuple<string, bool>> _ignoreList;

        //keeps track of whether the local List has had any changes made to it
        private static bool _dirtyIgnoreList;

        private static List<GameObject> _prefabsList;

        //keeps track of whether the local list has had any changes made to it
        private static bool _dirtyPrefabsList;

        // [SettingsProvider]
        // [PreferenceItem("Null Finder")]
        // [Obsolete("Obsolete")]
        // public static void PreferencesGUI()
        // {
        //     GUILayout.Space(15);
        //
        //     HandleAttributePreferences();
        //
        //     GUILayout.Space(15);
        //
        //     HandleIgnoreListPreferences();
        //
        //     GUILayout.Space(15);
        //
        //     HandlePrefabPreferences();
        // }

        #region attribute preferences

        /*do the UI elements for attribute tags*/
        private static void HandleAttributePreferences()
        {
            EditorGUILayout.LabelField("Attribute tag preferences");
            GUILayout.Space(5);

            var attributes = PreferencesStorage.PersistableAttributes;

            foreach (var attribute in attributes)
            {
                HandleIndividualAttribute(attribute);
            }
        }

        private static void HandleIndividualAttribute(PersistableAttribute attribute)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            attribute.IsEnabled = EditorGUILayout.Toggle(attribute.IsEnabled, GUILayout.Width(15));
            EditorGUILayout.LabelField(attribute.Identifier, GUILayout.Width(150));
            attribute.Color = EditorGUILayout.ColorField(attribute.Color, GUILayout.Width(500));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);
        }

        #endregion

        #region ignore-items preferences

        /*do the UI elements for the ignore list*/
        private static void HandleIgnoreListPreferences()
        {
            _ignoreList ??= ExtensionMethods.LoadIgnoreList();

            EditorGUILayout.LabelField(
                "Ignore list - enter the name of a GameObject to ignore when scanning (case sensitive)");
            GUILayout.Space(5);

            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, 10 + CellMargin);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 55 + CellMargin * 2f),
                new Color(0.5f, 0.5f, 0.5f,
                    0.3f)); //the drawn rectangle is bigger than the actual rectangle to include the buttons

            EditorGUI.LabelField(rect,
                "Use +/- buttons to increase or decrease number of items in ignore list: (" +
                _ignoreList.Count.ToString() + ")");

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                _ignoreList.Add(new Tuple<string, bool>("", false));
            }

            if (GUILayout.Button("-", GUILayout.Width(40)) && _ignoreList.Count > 0)
            {
                _ignoreList.RemoveAt(_ignoreList.Count - 1);
                _dirtyIgnoreList = true;
            }

            GUILayout.Space(18);

            if (_ignoreList.Count != 0)
            {
                //handle UI and save changes to local ignoreList
                for (var i = 0; i < _ignoreList.Count; i++)
                {
                    _ignoreList[i] = HandleIndividualIgnoreItem(_ignoreList[i]);
                }
            }

            //save the inputs, if anything changed
            if (_dirtyIgnoreList)
            {
                ExtensionMethods.SaveIgnoreList(_ignoreList);
                _dirtyIgnoreList = false;
            }
        }

        //Draws the UI element for each ignore item, and saves the values into the list for use
        private static Tuple<String, bool> HandleIndividualIgnoreItem(Tuple<String, bool> t)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));

            EditorGUILayout.LabelField("GameObject name: ", GUILayout.Width(112));
            var name = EditorGUILayout.TextField(t.Item1, GUILayout.Width(250));
            EditorGUILayout.LabelField("     Also ignore children: ", GUILayout.Width(137));
            var ignoreChildren = EditorGUILayout.Toggle(t.Item2, GUILayout.Width(15));
            //EditorGUILayout.LabelField(attribute.Identifier, GUILayout.Width(150));
            //attribute.Color = EditorGUILayout.ColorField(attribute.Color);            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);

            if (name != t.Item1 || ignoreChildren != t.Item2)
                _dirtyIgnoreList = true;

            return new Tuple<string, bool>(name.Trim(), ignoreChildren);
        }

        #endregion

        #region prefab-item preferences

        private static void HandlePrefabPreferences()
        {
            _prefabsList ??= ExtensionMethods.LoadPrefabList();

            EditorGUILayout.LabelField("Additional Prefabs to scan (only objects in the scene will be scanned," +
                                       " plus any prefabs which you define here)");
            GUILayout.Space(5);

            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, 10 + CellMargin);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 55 + CellMargin * 2f),
                new Color(0.5f, 0.5f, 0.5f,
                    0.3f)); //the drawn rectangle is bigger than the actual rectangle to include the buttons

            EditorGUI.LabelField(rect,
                "Use +/- buttons to increase or decrease number of items in external file list: (" +
                _prefabsList.Count.ToString() + ")");

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                _prefabsList.Add(null);
            }

            if (GUILayout.Button("-", GUILayout.Width(40)) && _prefabsList.Count > 0)
            {
                _prefabsList.RemoveAt(_prefabsList.Count - 1);
                _dirtyPrefabsList = true;
            }

            GUILayout.Space(18);

            //handle UI and save changes to local ignoreList
            if (_prefabsList.Count > 0)
            {
                for (var i = 0; i < _prefabsList.Count; i++)
                {
                    _prefabsList[i] = HandleIndividualPrefabItems(_prefabsList[i]);
                }
            }

            //save the inputs, if anything changed
            if (_dirtyPrefabsList)
            {
                ExtensionMethods.SavePrefabList(_prefabsList);
                _dirtyPrefabsList = false;
            }
        }

        private static GameObject HandleIndividualPrefabItems(GameObject previousValue)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));

            var newValue = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y + 7, 400, 15),
                "Select a prefab:",
                previousValue, typeof(GameObject), false);

            EditorGUILayout.LabelField("", GUILayout.Width(500));

            if (newValue != null)
                EditorGUILayout.LabelField(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(newValue));

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);

            if (newValue != previousValue)
            {
                _dirtyPrefabsList = true;
                return newValue;
            }
            else
            {
                return previousValue;
            }
        }

        #endregion
    }
}