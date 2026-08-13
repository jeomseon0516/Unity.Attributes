#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    internal sealed class EnumConstructorParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType.IsEnum;

        public object GetDefaultValue(Type parameterType)
        {
            Array values = Enum.GetValues(parameterType);
            return values.Length > 0 ? values.GetValue(0) : Activator.CreateInstance(parameterType);
        }

        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue)
        {
            Enum current = currentValue as Enum ?? (Enum)GetDefaultValue(parameterType);

            return parameterType.IsDefined(typeof(FlagsAttribute), false)
                ? EditorGUI.EnumFlagsField(position, label, current)
                : EditorGUI.EnumPopup(position, label, current);
        }
    }
}
#endif
