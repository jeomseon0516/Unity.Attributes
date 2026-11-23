#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class BoolConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(bool);
        public object GetDefaultValue(Type parameterType) => false;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.Toggle(position, label, currentValue is bool value && value);
        }

        public VisualElement CreateField(string label, Type parameterType, object currentValue, Action<object> onValueChanged)
        {
            Toggle field = new(label) { value = currentValue is bool typed && typed };
            field.RegisterValueChangedCallback(change => onValueChanged?.Invoke(change.newValue));
            return field;
        }
    }
}
#endif
