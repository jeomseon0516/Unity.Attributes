#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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

        public VisualElement CreateField(string label, Type parameterType, object currentValue, Action<object> onValueChanged)
        {
            TextField field = new(label) { value = currentValue as string ?? string.Empty };
            field.RegisterValueChangedCallback(change => onValueChanged?.Invoke(change.newValue));
            return field;
        }
    }
}
#endif
