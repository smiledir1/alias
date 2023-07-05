#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Common.UniTaskAnimations.Editor
{
    [CustomPropertyDrawer(typeof(SimpleTween), true)]
    public class SimpleTweenDrawer : IBaseTweenDrawer
    {
        #region Consts

        private static readonly int _buttonsCount = 4;
        private static string _cachedName = string.Empty;
        private new static float ButtonsHeight => _buttonHeight * 2;

        #endregion

        private GameObject _tweenObject;

        private bool _initialized;
        private readonly PopupTypes _inheredTypes = new();
        private int _popupTweenIndex;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Initialize();

            string tweenName;
            if (property.managedReferenceId == -1 ||
                property.managedReferenceValue == null)
            {
                property.isExpanded = true;
                tweenName = "Null";
            }
            else
            {
                tweenName = property.managedReferenceValue.GetType().ToString();
            }

            var propertyYAdd = 0f;
            if (property.isExpanded)
            {
                if (_tweenObject == null)
                {
                    _tweenObject = property.serializedObject.targetObject switch
                    {
                        GameObject go => go,
                        Component component => component.gameObject,
                        _ => _tweenObject
                    };
                }

                DrawChooseTween(rect, property);
                DrawSetTweenButtons(rect, property);
                propertyYAdd = ButtonsHeight;
            }

            var lastIndexOfPoint = tweenName.LastIndexOf('.');
            var shortTweenNameLength = tweenName.Length - lastIndexOfPoint - 1;
            var shortTweenName = tweenName.Substring(lastIndexOfPoint + 1, shortTweenNameLength);

            if (string.Equals(_cachedName, shortTweenName, StringComparison.Ordinal))
            {
                label.text = _cachedName;
            }
            else
            {
                label.text += $" {shortTweenName}";
                _cachedName = label.text;
            }

            var propertyRect = new Rect(rect.x, rect.y + propertyYAdd, rect.width, rect.height);
            EditorGUI.PropertyField(propertyRect, property, label, true);

            if (GUI.changed && property.managedReferenceValue is SimpleTween simpleTween)
            {
                OnGuiChange(simpleTween).Forget();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded
                ? EditorGUI.GetPropertyHeight(property) + ButtonsHeight + Space
                : EditorGUI.GetPropertyHeight(property);
        }

        private void DrawSetTweenButtons(Rect propertyRect, SerializedProperty property)
        {
            var buttonWidth = propertyRect.width / _buttonsCount;
            var x = propertyRect.x;
            var y = propertyRect.yMin + _buttonHeight;

            var buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Copy"))
            {
                CachedTween = property.managedReferenceValue as IBaseTween;
            }

            x = propertyRect.x + buttonWidth;
            buttonRect = new Rect(x, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Paste"))
            {
                var currentTween = property.managedReferenceValue as SimpleTween;
                var targetGo = currentTween?.TweenObject;
                if (targetGo == null)
                {
                    var component = property.serializedObject?.targetObject as Component;
                    if (component != null) targetGo = component.gameObject;
                }

                var cloneTween = IBaseTween.Clone(CachedTween, targetGo);
                property.managedReferenceValue = cloneTween;
            }
        }

        private class PopupTypes
        {
            private readonly List<Type> _types = new();
            public string[] Names;

            public void Clear()
            {
                _types.Clear();
                Names = null;
            }

            public void Add(Type type)
            {
                _types.Add(type);
            }

            public void Compile()
            {
                Names = new string[_types.Count + 1];
                Names[0] = "Null";
                for (var i = 0; i < _types.Count; i++)
                {
                    var type = _types[i];
                    Names[i + 1] = type.Name;
                }
            }

            public string this[int index] => index < 1 ? "Null" : _types[index - 1].Name;
        }

        private void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _inheredTypes.Clear();
            foreach (var type in
                     Assembly.GetAssembly(typeof(SimpleTween)).GetTypes()
                         .Where(myType => myType.IsClass &&
                                          !myType.IsAbstract &&
                                          myType.IsSubclassOf(typeof(SimpleTween))))
            {
                _inheredTypes.Add(type);
            }

            _inheredTypes.Compile();
        }

        private void DrawChooseTween(Rect propertyRect, SerializedProperty property)
        {
            var y = propertyRect.yMin;
            var x = propertyRect.x;

            var labelWidth = propertyRect.width * 7 / 32;
            var labelRect = new Rect(x, y, labelWidth, _buttonHeight);
            EditorGUI.LabelField(labelRect, "Tween:");

            var popupX = x + labelWidth;
            var popupWidth = propertyRect.width * 3 / 8;
            var popupRect = new Rect(popupX, y, popupWidth, _buttonHeight);
            _popupTweenIndex = EditorGUI.Popup(popupRect, _popupTweenIndex, _inheredTypes.Names);

            var spaceX = popupX + popupWidth;
            var spaceWidth = propertyRect.width / 32;
            var spaceRect = new Rect(spaceX, y, spaceWidth, _buttonHeight);
            EditorGUI.LabelField(spaceRect, " ");

            var buttonX = spaceX + spaceWidth;
            var buttonWidth = propertyRect.width * 3 / 8;
            var buttonRect = new Rect(buttonX, y, buttonWidth, _buttonHeight);
            if (GUI.Button(buttonRect, "Set"))
            {
                SetType(_inheredTypes[_popupTweenIndex], property);
            }
        }

        private void SetType(string typeName, SerializedProperty property)
        {
            var tween = TweenFactory.CreateSimpleTween(typeName, _tweenObject);

            if (tween != null)
            {
                if (property.managedReferenceId == -1)
                {
                    Debug.Log("Shoud Be SerializeReference");
                }
                else
                {
                    property.managedReferenceValue = tween;
                }
            }
            else
            {
                property.managedReferenceValue = null;
            }
        }

        private async UniTask OnGuiChange(SimpleTween simpleTween)
        {
            await UniTask.Yield();
            simpleTween.OnGuiChange();
        }
    }
}
#endif