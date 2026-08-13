#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class StringConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(string);
        public object GetDefaultValue(Type parameterType) => string.Empty;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.TextField(position, label, currentValue as string ?? string.Empty);
        }
    }
}
#endif
