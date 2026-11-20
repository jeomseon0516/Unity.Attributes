#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(Vector2SliderAttribute), true)]
    internal sealed class Vector2SliderDrawer : PropertyDrawer
    {
        // EditorGUI.indentLevel 한 단계와 같은 들여쓰기입니다.
        private const float IndentWidth = 15f;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
                return new PropertyField(property);

            Vector2SliderAttribute range = (Vector2SliderAttribute)attribute;
            VisualElement root = new();
            root.Add(new Label(property.displayName));
            root.Add(CreateAxisSlider("X", property.FindPropertyRelative("x"), range));
            root.Add(CreateAxisSlider("Y", property.FindPropertyRelative("y"), range));
            return root;
        }

        private static Slider CreateAxisSlider(string label, SerializedProperty axis, Vector2SliderAttribute range)
        {
            Slider slider = new(label, range.Min, range.Max)
            {
                showInputField = true
            };
            slider.AddToClassList(BaseField<float>.alignedFieldUssClassName);
            slider.style.marginLeft = IndentWidth;
            slider.BindProperty(axis);
            return slider;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2SliderAttribute range = (Vector2SliderAttribute)attribute;

            Rect labelRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            Rect sliderRectX = new(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            Rect sliderRectY = new(position.x, sliderRectX.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

            Vector2 vector = property.vector2Value;

            EditorGUI.indentLevel++;
            vector.x = EditorGUI.Slider(sliderRectX, "X", vector.x, range.Min, range.Max);
            vector.y = EditorGUI.Slider(sliderRectY, "Y", vector.y, range.Min, range.Max);
            EditorGUI.indentLevel--;

            property.vector2Value = vector;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}
#endif
