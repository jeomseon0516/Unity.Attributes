#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MaxValueAttribute), true)]
    internal sealed class MaxValueAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            MaxValueAttribute maxAttribute = (MaxValueAttribute)attribute;
            VisualElement root = new();

            if (property.propertyType == SerializedPropertyType.Float)
            {
                FloatField field = new(property.displayName)
                {
                    bindingPath = property.propertyPath
                };
                field.RegisterValueChangedCallback(change =>
                {
                    float clamped = Clamp(change.newValue, maxAttribute.Max);
                    if (!Mathf.Approximately(change.newValue, clamped))
                        field.SetValueWithoutNotify(clamped);
                    property.floatValue = clamped;
                    property.serializedObject.ApplyModifiedProperties();
                });
                root.Add(field);
                return root;
            }

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                IntegerField field = new(property.displayName)
                {
                    bindingPath = property.propertyPath
                };
                field.RegisterValueChangedCallback(change =>
                {
                    int clamped = Clamp(change.newValue, maxAttribute.Max);
                    if (change.newValue != clamped)
                        field.SetValueWithoutNotify(clamped);
                    property.intValue = clamped;
                    property.serializedObject.ApplyModifiedProperties();
                });
                root.Add(field);
                return root;
            }

            root.Add(new PropertyField(property));
            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MaxValueAttribute maxAttribute = (MaxValueAttribute)attribute;

            EditorGUI.BeginChangeCheck();
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    {
                        float value = EditorGUI.FloatField(position, label, property.floatValue);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.floatValue = Clamp(value, maxAttribute.Max);
                        }
                        break;
                    }
                case SerializedPropertyType.Integer:
                    {
                        int value = EditorGUI.IntField(position, label, property.intValue);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.intValue = Clamp(value, maxAttribute.Max);
                        }
                        break;
                    }
                default:
                    EditorGUI.PropertyField(position, property, label, true);
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        internal static float Clamp(float value, float max)
        {
            return Mathf.Min(value, max);
        }

        internal static int Clamp(int value, float max)
        {
            return Mathf.Min(value, Mathf.FloorToInt(max));
        }
    }
}
#endif
