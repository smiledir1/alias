using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Common.Extensions;
using Cysharp.Threading.Tasks;
using Game.UI.Popups.Confirm;
using Game.UI.Popups.Message;
using Game.UserData;
using Services.Assets;
using Services.Helper;
using Services.UI;
using Services.UI.PopupService;
using Services.UserData;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//Test Script for debug
public class Test : MonoBehaviour
{
    [Service]
    private static IAssetsService _assetsService;
    
    [Service]
    private static IPopupService _popupService;
    
    public int number = 0;
    
    public float number2 = 0;
    
    public float round = 0;

    // public A SimpleA;
    // public B SimpleB;
    // public C SimpleC;
    //
    // [SerializeReference]
    // public A SimpleARef;
    //
    // [SerializeReference]
    // public B SimpleBRef;
    //
    // [SerializeReference]
    // public C SimpleCRef;
    //
    // [SerializeReference]
    // public A SimpleARef2 = new B();
    //
    // [SerializeReference]
    // public B SimpleBRef2 = new B();
    //
    // [SerializeReference]
    // public C SimpleCRef2 = new C();

    // public class Abc
    // {
    //     public event Action TestAction;
    // }
    //

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeCode()
    {
        Debug.Log("InitializeCode");
    }

    public void OpenMessagePopup()
    {
        var messagePopupModel = new MessagePopupModel("message" ,"tttterfdgdf");
        _popupService.ShowAsync<MessagePopup>(messagePopupModel);
        WaitMessageClose(messagePopupModel).Forget();
        
    }

    private async UniTask WaitMessageClose(UIModel messagePopupModel)
    {
        await messagePopupModel.WaitForClose();
        Debug.Log("Message Close");
    }

    public void OpenConfirmPopup()
    {
        var messagePopupModel = new ConfirmPopupModel("Are you sure?", "Some special text");
        _popupService.ShowAsync<ConfirmPopup>(messagePopupModel);
    }

    private void OnValidate()
    {
        if (number == 10)
        {
            var hasValue = _assetsService == null;
            Debug.Log(hasValue);
        }

        //ShowRound();
        //if (number != 5) return;
        // if (number == 7)
        // {
        //     _b.ShowEvents();
        // }
        //
        // if (number == 6)
        // {
        //     TestActions();
        // }

        //TestUserData();
    }

    private void ShowRound()
    {
        Debug.Log(number2.ToString($"F{round}"));
    }

    private UserDataService _userDataService;
    private void TestUserData()
    {
        if (number == 5)
        {
            _userDataService = new UserDataService(new List<UserDataObject>()
            {
                new SettingsUserData()
            }, false);
            _userDataService.Initialize().Forget();
        }
        
        if (number == 6)
        {
            _userDataService.SaveUserData();
        }

        if (number == 7)
        {
            var settings = _userDataService.GetData<SettingsUserData>();
            settings.VibrationEnabled = !settings.VibrationEnabled;
            Debug.Log($"vv: {settings.VibrationEnabled}");
        }
    }
    
    // private B _b;
    // private void TestActions()
    // {
    //     _b = new B();
    //     _b.AAA += () => Deleg1();
    //     _b.AAA += () => Deleg2();
    // }
    //
    // private void Deleg1()
    // {
    //     Debug.Log("Deleg1");
    // }
    //
    // private void Deleg2()
    // {
    //     Debug.Log("Deleg2");
    // }
    
    [CustomPropertyDrawer(typeof(A), true)]
    public class ADrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            // var context = property.serializedObject.context;
            // var iterator = property.serializedObject.GetIterator();
            // foreach (var iter in iterator)
            // {
            //     var propertyyy = iter as SerializedProperty;
            //     Debug.Log($"enter {propertyyy.type} {propertyyy.propertyType}");
            //     
            //     if (iter == null)
            //     {
            //         Debug.Log("null");
            //     }
            //     
            //     if (propertyyy.type == "A")
            //     {
            //         Debug.Log("type A");
            //     }
            // }
            if (GUI.Button(new Rect(position.xMin + 50f, position.yMax - 20f, position.width - 100f, 20f), "button"))
            {
                if (property.managedReferenceId == -1)
                {
                    Debug.Log("Shoud Be SerializeReference");
                    return;
                }

                var reff = property.managedReferenceValue;
                property.managedReferenceValue = new B();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) + 20f;
        }
    }

    [Serializable]
    public abstract class A
    {
        public int Num = 10;
        public string Aza = "yyy";

        // public event Action AAA;
        // public event Action BBB;

        // public void ShowEvents()
        // {
        //     AAA?.Invoke();
        // }
    }

    [Serializable]
    public class B : A
    {
        public string Haa = "ooo";
    }

    [Serializable]
    public class C : A
    {
        public string Mmm = "mmm";
    }

    #region Attributes

    private void TestAttributes2()
    {
        var assetsService = new AssetsService();
        var attributeType = typeof(ServiceAttribute);
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        // foreach (var assembly in assemblies)
        // {
        //     var types = assembly.GetTypes();
        //     foreach (var type in types)
        //     {
        //         var hasServiceAttribute = type.HasAttribute(attributeType);
        //         if (hasServiceAttribute)
        //         {
        //             Debug.Log($"Type: {type}");
        //             var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
        //             foreach (var fieldInfo in fields)
        //             {
        //                 var attributes = fieldInfo.GetCustomAttributes(attributeType);
        //                 foreach (var attribute in attributes)
        //                 {
        //                     Debug.Log(attribute);
        //                 }
        //
        //                 var isCustom = fieldInfo.HasAttribute(attributeType);
        //                 if (isCustom)
        //                 {
        //                     var fieldType = fieldInfo.FieldType;
        //                     if (fieldType == typeof(IAssetsService))
        //                     {
        //                         fieldInfo.SetValue(null, assetsService);
        //                         Debug.Log("add");
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     // foreach (var customAttribute in assembly.CustomAttributes)
        //     // {
        //     //     if (customAttribute.AttributeType == attributeType)
        //     //     {
        //     //         
        //     //     }
        //     // }
        //
        //     //
        //     // var serviceAttributes = Attribute.GetCustomAttributes(assembly, typeof(ServiceAttribute));
        //     // if (serviceAttributes.Length > 0)
        //     // {
        //     //     foreach (var serviceAttribute in serviceAttributes)
        //     //     {
        //     //         Debug.Log(serviceAttribute);
        //     //     }
        //     // }
        // }
        var mainType = typeof(MonoBehaviour);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsSubclassOf(mainType)) continue;
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var fieldInfo in fields)
                {
                    var attributes = fieldInfo.GetCustomAttributes(attributeType);
                    foreach (var attribute in attributes)
                    {
                        Debug.Log(attribute);
                    }

                    var isCustom = fieldInfo.HasAttribute(attributeType);
                    if (isCustom)
                    {
                        var fieldType = fieldInfo.FieldType;
                        if (fieldType == typeof(IAssetsService))
                        {
                            fieldInfo.SetValue(null, assetsService);
                            Debug.Log("add");
                        }
                    }
                }
            }
        }

    }

    private void TestAttributesAndProperties()
    {
        if (number != 5) return;
        var assembly = this.GetType().Assembly;
        var attrs = Attribute.GetCustomAttributes(assembly, typeof(CustomAttribute));
        var attrs2 = Attribute.GetCustomAttributes(assembly);

        Debug.Log("attr");
        foreach (var attr in attrs)
        {
            Debug.Log(attr);
        }

        Debug.Log("attr2");
        foreach (var attr in attrs2)
        {
            Debug.Log(attr);
        }


        var properties = this.GetType().GetProperties(BindingFlags.Static & BindingFlags.NonPublic);
        var properties4 = this.GetType().GetProperties(BindingFlags.Static | BindingFlags.NonPublic);
        var properties2 = this.GetType().GetProperties(BindingFlags.Static);
        var properties3 = this.GetType().GetProperties();
        var fields = this.GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic);
        Debug.Log("attr3");
        foreach (var property in properties4)
        {
            var customAttributes = property.GetCustomAttributes(typeof(CustomAttribute));
            foreach (var customAttribute in customAttributes)
            {
                Debug.Log(customAttribute);
            }

            var isCustom = property.HasAttribute<CustomAttribute>();
            if (isCustom)
            {
                var number2 = 20;
                var t = number2;
                property.SetValue(null, t);
            }
        }

        Debug.Log("attr4");
        foreach (var fieldInfo in fields)
        {
            var customAttributes = fieldInfo.GetCustomAttributes(typeof(CustomAttribute));
            foreach (var customAttribute in customAttributes)
            {
                Debug.Log(customAttribute);
            }

            var isCustom = fieldInfo.HasAttribute<CustomAttribute>();
            if (isCustom)
            {
                var number2 = 20;
                var t = number2;
                var type = fieldInfo.FieldType;
                fieldInfo.SetValue(null, t);
            }
        }
    }

    #endregion
}

[AttributeUsage(AttributeTargets.Field)]
public class CustomAttribute : Attribute
{
}