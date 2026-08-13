#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class FloatConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(float);
        public object GetDefaultValue(Type parameterType) => 0f;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.FloatField(position, label, currentValue is float value ? value : 0f);
        }
    }
}
#endif
