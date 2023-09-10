using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Common.CheckProject.Editor.NullReferenceDetection
{
    public static class ExtensionMethods
    {
        public static IEnumerable<Component> AllComponents(this GameObject gameObject) =>
            gameObject.GetComponents<Component>();

        public static IEnumerable<FieldInfo> GetInspectableFields(this Component component)
        {
            var type = component.GetType();
            var inspectableFields = new List<FieldInfo>();

            var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            var privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            //Private fields can be inspected if they are explicitly serialized
            privateFields = privateFields.Where(f => f.HasAttribute<SerializeField>()).ToList();
            //Add remaining private and public fields to the list of all inspectable fields
            inspectableFields.AddRange(publicFields);
            inspectableFields.AddRange(privateFields);
            //Remove fields that should be hidden in the inspector
            inspectableFields = inspectableFields.Where(f => !f.HasAttribute<HideInInspector>()).ToList();

            return inspectableFields;
        }

        public static bool HasAttribute<T>(this FieldInfo field) => field.GetCustomAttributes(typeof(T), true).Any();

        public static T GetAttribute<T>(this FieldInfo field) =>
            (T) field.GetCustomAttributes(typeof(T), true).FirstOrDefault();

        public static bool IsNull(this FieldInfo field, object obj)
        {
            var value = field.GetValue(obj);
            return value == null || value.ToString() == "null";
        }

        public static IEnumerable<Type> GetDescendantTypes(this Type type)
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            return allTypes.Where(t => t != type && type.IsAssignableFrom(t)).ToList();
        }

        #region save/load functions

        private const string FolderPath = ""; //TODO where would we like to save the files?
        private static readonly string IgnoreListfile = FolderPath + "NullReferenceDetector.IgnoreList";
        private static readonly string PrefabListFile = FolderPath + "NullReferenceDetector.PrefabList";

        //applies the values entered in the GUI + saves them  for future sessions
        public static void SaveIgnoreList(List<Tuple<string, bool>> ignoreList)
        {
            ignoreList = ignoreList.Where(tuple => tuple.Item1.Length != 0).ToList(); //remove any empty entries

            try
            {
                TextWriter tr = new StreamWriter(IgnoreListfile, false);

                foreach (var t in ignoreList)
                {
                    var children = t.Item2 ? "1" : "0";
                    tr.WriteLine(t.Item1 + " " + children);
                }

                tr.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        //reads values from file and returns list
        public static List<Tuple<string, bool>> LoadIgnoreList()
        {
            var ignoreList = new List<Tuple<string, bool>>();

            if (!File.Exists(IgnoreListfile))
            {
                File.Create(IgnoreListfile);
                Debug.LogWarning("Null reference checker found no previously saved preferences file," +
                                 " so it created a new one");
                return ignoreList;
            }

            try
            {
                TextReader tr =
                    new StreamReader(IgnoreListfile);

                while (tr.Peek() != -1)
                {
                    var line = tr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.Trim().Length == 0) continue;

                    var name = line.Substring(0, line.Length - 2);

                    switch (line[^1])
                    {
                        case '1':
                            ignoreList.Add(new Tuple<string, bool>(name, true));
                            break;
                        case '0':
                            ignoreList.Add(new Tuple<string, bool>(name, false));
                            break;
                        default:
                            throw new IOException(
                                "Null Reference Checker encountered a problem when loading saved file");
                    }
                }

                tr.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }

            return ignoreList;
        }


        //applies the values entered in the GUI + saves them  for future sessions
        public static void SavePrefabList(List<GameObject> prefabList)
        {
            try
            {
                TextWriter tw =
                    new StreamWriter(PrefabListFile, false);

                foreach (var t in prefabList)
                {
                    if (t != null)
                        tw.WriteLine(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t));
                }

                tw.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        //reads values from file and returns list
        //when saved to file the paths of the prefabs are saved, not the prefabs themselves
        public static List<GameObject> LoadPrefabList()
        {
            var results = new List<GameObject>();

            if (!File.Exists(PrefabListFile))
            {
                File.Create(PrefabListFile);
                Debug.LogWarning(
                    "Null reference checker found no previously saved preferences file, so it created a new one");
                return new List<GameObject>();
            }

            try
            {
                TextReader tr =
                    new StreamReader(PrefabListFile);

                while (tr.Peek() != -1)
                {
                    var line = tr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.Trim().Length == 0) continue;

                    var g = AssetDatabase.LoadAssetAtPath<GameObject>(line);

                    if (g != null)
                        results.Add(g);
                    else
                        Debug.LogWarning(
                            "Null reference checker found no prefab at path " + line +
                            " perhaps the path or filename changed. this path will be deleted from the list of prefabs and needs to be re-added manually");
                }

                tr.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }

            return results;
        }

        #endregion
    }
}