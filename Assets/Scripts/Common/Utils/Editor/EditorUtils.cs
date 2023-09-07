using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Utils.Editor
{
    public static class EditorUtils
    {
        public static bool DrawList(IList<string> stringList, bool isExpanded = true, string name = "List")
        {
            if (isExpanded)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(name);
                if (GUILayout.Button("Hide")) isExpanded = false;
                GUILayout.EndHorizontal();

                for (var i = 0; i < stringList.Count; i++)
                {
                    stringList[i] = EditorGUILayout.TextField(stringList[i]);
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+")) stringList.Add(string.Empty);
                if (GUILayout.Button("-")) stringList.RemoveAt(stringList.Count - 1);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(name);
                if (GUILayout.Button("Show")) isExpanded = true;
                GUILayout.EndHorizontal();
            }

            return isExpanded;
        }
    }
}