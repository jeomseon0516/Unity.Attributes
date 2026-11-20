using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    internal sealed class ShowIfDrawer : PropertyDrawer
    {
        internal static SerializedProperty FindCondition(SerializedProperty property, string memberName)
        {
            var separator = property.propertyPath.LastIndexOf('.');
            var prefix = separator < 0 ? string.Empty : property.propertyPath.Substring(0, separator + 1);
            var serializedObject = property.serializedObject;
            return serializedObject.FindProperty(prefix + memberName)
                ?? serializedObject.FindProperty(prefix + "<" + memberName + ">k__BackingField");
        }

        internal static bool IsVisible(SerializedProperty condition, int expected)
        {
            // Invalid conditions stay visible so configuration errors cannot hide data.
            if (condition == null || condition.hasMultipleDifferentValues)
                return true;

            return condition.propertyType switch
            {
                SerializedPropertyType.Boolean => condition.boolValue == (expected != 0),
                SerializedPropertyType.Enum => condition.intValue == expected,
                SerializedPropertyType.Integer => condition.intValue == expected,
                _ => true
            };
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new PropertyField(property);
            var rule = (ShowIfAttribute)attribute;
            var condition = FindCondition(property, rule.MemberName);
            field.style.display = IsVisible(condition, rule.ExpectedValue) ? DisplayStyle.Flex : DisplayStyle.None;
            if (condition != null)
            {
                field.TrackPropertyValue(condition, changed =>
                    field.style.display = IsVisible(changed, rule.ExpectedValue) ? DisplayStyle.Flex : DisplayStyle.None);
            }
            return field;
        }

        private bool IsVisible(SerializedProperty property)
        {
            var rule = (ShowIfAttribute)attribute;
            return IsVisible(FindCondition(property, rule.MemberName), rule.ExpectedValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => IsVisible(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : -EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsVisible(property))
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
