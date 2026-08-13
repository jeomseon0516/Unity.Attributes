#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class IntConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(int);
        public object GetDefaultValue(Type parameterType) => 0;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.IntField(position, label, currentValue is int value ? value : 0);
        }
    }
}
#endif
