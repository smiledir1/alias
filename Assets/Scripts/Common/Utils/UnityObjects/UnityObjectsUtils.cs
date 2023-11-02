using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.Utils.UnityObjects
{
    public static class UnityObjectsUtils
    {
        public static void DestroyObjects(List<GameObject> objectList)
        {
            foreach (var item in objectList)
            {
                Object.Destroy(item);
            }

            objectList.Clear();
        }

        public static void DestroyObjects<T>(List<T> objectList) where T : Component
        {
            foreach (var item in objectList)
            {
                Object.Destroy(item.gameObject);
            }

            objectList.Clear();
        }

        public static void CreateItems<TComponent, TModel>(
            TComponent templateItem,
            List<TComponent> itemsList,
            IReadOnlyList<TModel> modelsList,
            Action<TComponent, int> addItemCallback = null,
            Transform parent = null)
            where TComponent : Component
        {
            templateItem.gameObject.SetActive(false);

            var curParent = parent == null ? templateItem.transform.parent : parent;
            for (var i = 0; i < modelsList.Count; i++)
            {
                var newItem = Object.Instantiate(templateItem, curParent);
                itemsList.Add(newItem);
                addItemCallback?.Invoke(newItem, i);
                newItem.gameObject.SetActive(true);
            }
        }

        public static void CreateItemsAndInitialize<TComponent, TModel>(
            TComponent templateItem,
            List<TComponent> itemsList,
            IReadOnlyList<TModel> modelsList,
            Action<TComponent, int> addItemCallback = null,
            Transform parent = null)
            where TComponent : Component, IInitializable<TModel>
        {
            templateItem.gameObject.SetActive(false);

            var curParent = parent == null ? templateItem.transform.parent : parent;
            for (var i = 0; i < modelsList.Count; i++)
            {
                var newItem = Object.Instantiate(templateItem, curParent);
                itemsList.Add(newItem);
                var model = modelsList[i];
                newItem.Initialize(model);
                addItemCallback?.Invoke(newItem, i);
                newItem.gameObject.SetActive(true);
            }
        }

        public static void DestroyCreateItems<TComponent, TModel>(
            TComponent templateItem,
            List<TComponent> itemsList,
            IReadOnlyList<TModel> modelsList,
            Action<TComponent, int> addItemCallback = null,
            Transform parent = null)
            where TComponent : Component
        {
            DestroyObjects(itemsList);
            CreateItems(templateItem, itemsList, modelsList, addItemCallback, parent);
        }

        public static void DestroyCreateInitializeItems<TComponent, TModel>(
            TComponent templateItem,
            List<TComponent> itemsList,
            IReadOnlyList<TModel> modelsList,
            Action<TComponent, int> addItemCallback = null,
            Transform parent = null)
            where TComponent : Component, IInitializable<TModel>
        {
            DestroyObjects(itemsList);
            CreateItemsAndInitialize(templateItem, itemsList, modelsList, addItemCallback, parent);
        }
    }

    public interface IInitializable<TModel>
    {
        void Initialize(TModel model);
    }
}