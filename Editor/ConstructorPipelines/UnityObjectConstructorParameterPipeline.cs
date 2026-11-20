#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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

        public VisualElement CreateField(string label, Type parameterType, object currentValue, Action<object> onValueChanged)
        {
            ObjectField field = new(label)
            {
                objectType = parameterType,
                allowSceneObjects = true,
                value = currentValue as UnityEngine.Object
            };
            field.RegisterValueChangedCallback(change => onValueChanged?.Invoke(change.newValue));
            return field;
        }
    }
}
#endif
