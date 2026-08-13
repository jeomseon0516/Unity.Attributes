#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class Vector2ConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(Vector2);
        public object GetDefaultValue(Type parameterType) => Vector2.zero;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.Vector2Field(position, label, currentValue is Vector2 value ? value : Vector2.zero);
        }
    }
}
#endif
