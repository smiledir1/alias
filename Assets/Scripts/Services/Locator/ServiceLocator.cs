using System;
using System.Collections.Generic;
using System.Reflection;
using Common.StateMachine;
using Cysharp.Threading.Tasks;
using Services.Common;
using Services.Helper;
using UnityEngine;

namespace Services.Locator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> Services = new();
        private static readonly List<IService> AllServices = new();

        public static void AddService<T>(IService service) where T : IService
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError($"Invalid type. {typeof(T).Name} should be an interface");
                return;
            }

            Services.Add(typeof(T), service);
            AllServices.Add(service);
        }

        public static T GetService<T>() where T : IService
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError($"Invalid type. {typeof(T).Name} should be an interface");
                return default;
            }

            if (Services.TryGetValue(typeof(T), out var service)) return (T) service;

            Debug.LogError($"Service of type \"{typeof(T).Name}\" is not found!");
            return default;
        }

        public static async UniTask RemoveAndDisposeService<T>() where T : IService
        {
            if (!Services.TryGetValue(typeof(T), out var serviceInterface))
            {
                Debug.LogError($"Service of type \"{typeof(T).Name}\" is not found!");
                return;
            }

            var service = serviceInterface as Service;
            Services.Remove(typeof(T));
            if (service != null)
            {
                await service.Dispose();
                AllServices.Remove(service);
            }
        }

        public static async UniTask InitializeServices()
        {
            var tasks = new List<UniTask>();

            foreach (var service in AllServices)
            {
                if (service.State != ServiceState.Created) continue;
                tasks.Add(service.Initialize());
            }

            await UniTask.WhenAll(tasks);
        }

        public static void FillServices()
        {
            // Check for performance
            // TODO: make list
            var monoBehaviourType = typeof(MonoBehaviour);
            var stateType = typeof(GameState);
            var iServices = typeof(IHasServices);

            var attributeType = typeof(ServiceAttribute);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(monoBehaviourType) &&
                        !type.IsSubclassOf(stateType) &&
                        !type.IsSubclassOf(iServices)) continue;

                    var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                    foreach (var fieldInfo in fields)
                    {
                        var isCustom = false;
                        foreach (var attributeData in fieldInfo.CustomAttributes)
                        {
                            if (attributeData.AttributeType != attributeType) continue;
                            isCustom = true;
                            break;
                        }

                        if (isCustom)
                        {
                            var fieldType = fieldInfo.FieldType;
                            if (!Services.TryGetValue(fieldType, out var service))
                            {
                                Debug.Log($"Not Added Service: {fieldType} on Type: {type}");
                                continue;
                            }

                            fieldInfo.SetValue(null, service);
                        }
                    }
                }
            }
        }

        public static async UniTask DisposeServices()
        {
            var tasks = new List<UniTask>();

            foreach (var service in AllServices)
            {
                if (service.State != ServiceState.Initialized) continue;
                tasks.Add(service.Dispose());
            }

            await UniTask.WhenAll(tasks);
        }
    }
}