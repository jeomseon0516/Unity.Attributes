#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class ColorConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(Color);
        public object GetDefaultValue(Type parameterType) => Color.white;

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            return EditorGUI.ColorField(position, label, currentValue is Color value ? value : Color.white);
        }

        public VisualElement CreateField(string label, Type parameterType, object currentValue, Action<object> onValueChanged)
        {
            ColorField field = new(label) { value = currentValue is Color typed ? typed : Color.white };
            field.RegisterValueChangedCallback(change => onValueChanged?.Invoke(change.newValue));
            return field;
        }
    }
}
#endif
