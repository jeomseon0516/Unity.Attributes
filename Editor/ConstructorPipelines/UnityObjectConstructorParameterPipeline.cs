#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class UnityObjectConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => typeof(UnityEngine.Object).IsAssignableFrom(parameterType);
        public object GetDefaultValue(Type parameterType) => null;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.ObjectField(position, label, currentValue as UnityEngine.Object, parameterType, true);
        }
    }
}
#endif
