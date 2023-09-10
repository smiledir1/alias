using System;
using System.Reflection;
using UnityEngine;

namespace Common.CheckProject.Editor.NullReferenceDetection
{
    public struct NullReference
    {
        public static readonly string UnattributedIdentifier = "Unattributed";

        public Type Attribute;
        public FieldInfo FieldInfo;
        public Component Source;

        public GameObject GameObject => Source.gameObject;

        public string GameObjectName => Source.gameObject.name;

        public string ComponentName => Source.GetType().ToString();

        public string FieldName => FieldInfo.Name;

        public bool IsAttributed => Attribute != null;

        public string AttributeIdentifier => Attribute != null ? Attribute.Name : UnattributedIdentifier;
    }
}