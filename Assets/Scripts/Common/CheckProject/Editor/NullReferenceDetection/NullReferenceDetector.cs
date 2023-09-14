using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.CheckProject.Editor.NullReferenceDetection.Attributes;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Common.CheckProject.Editor.NullReferenceDetection
{
    public class NullReferenceDetector
    {
        public IEnumerable<NullReference> FindAllNullReferences(List<Tuple<string, bool>> ignoreList,
            List<GameObject> prefabList)
        {
            //var sceneObjects = UnityObject.FindObjectsOfType<GameObject>();
            //var allObjects = sceneObjects.Concat(prefabObjects).ToArray();

            var prefabObjects = FindObjectsInPrefabs(prefabList);

            prefabObjects = RemoveIgnoredItems(prefabObjects, ignoreList);
            return prefabObjects.SelectMany(FindNullReferencesIn);
        }

        public IEnumerable<NullReference> FindAllNullReferences(Func<NullReference, bool> filter,
            List<Tuple<string, bool>> ignoreList, List<GameObject> prefabList) =>
            FindAllNullReferences(ignoreList, prefabList).Where(filter).ToList();

        private IEnumerable<NullReference> FindNullReferencesIn(GameObject gameObject)
        {
            var components = gameObject.AllComponents();
            return components.Where(n => n != null).SelectMany(FindNullReferencesIn);
        }

        private IEnumerable<NullReference> FindNullReferencesIn(Component component)
        {
            var inspectableFields = component.GetInspectableFields();
            var nullFields = inspectableFields.Where(f => f.IsNull(component));
            return nullFields.Select(f => new NullReference
                {Source = component, FieldInfo = f, Attribute = AttributeFor(f)});
        }

        private Type AttributeFor(FieldInfo field)
        {
            if (field.HasAttribute<BaseAttribute>()) return field.GetAttribute<BaseAttribute>().GetType();

            return null;
        }

        private GameObject[] RemoveIgnoredItems(GameObject[] objects, List<Tuple<string, bool>> ignoreList)
        {
            var objectsToBeRemoved = new List<GameObject>();

            ////1. make a list of gameobjects to be filtered out (not really efficient but it doesnt need to be 👿)
            foreach (var tuple in ignoreList)
            {
                foreach (var g in objects)
                {
                    if (g.name != tuple.Item1) continue;
                    objectsToBeRemoved.Add(g);

                    if (!tuple.Item2) continue;
                    foreach (Transform t in g.transform)
                    {
                        objectsToBeRemoved.Add(t.gameObject);
                    }
                }
            }

            ////2. now remove those items we found

            var cleanedList = objects.ToList();

            foreach (var g in objectsToBeRemoved)
            {
                cleanedList.Remove(g);
            }

            return cleanedList.ToArray();
        }

        private GameObject[] FindObjectsInPrefabs(List<GameObject> prefabList)
        {
            var foundObjects = new List<GameObject>();

            foreach (var gameObject in prefabList)
            {
                foundObjects.Add(gameObject);
                foundObjects.AddRange(gameObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
            }

            return foundObjects.ToArray();
        }
    }
}