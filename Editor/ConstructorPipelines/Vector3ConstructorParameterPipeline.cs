#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class Vector3ConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(Vector3);
        public object GetDefaultValue(Type parameterType) => Vector3.zero;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.Vector3Field(position, label, currentValue is Vector3 value ? value : Vector3.zero);
        }
    }
}
#endif
