#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class Vector4ConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(Vector4);
        public object GetDefaultValue(Type parameterType) => Vector4.zero;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.Vector4Field(position, label, currentValue is Vector4 value ? value : Vector4.zero);
        }
    }
}
#endif
