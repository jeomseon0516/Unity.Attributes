#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Jeomseon.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(MaxValueAttribute), true)]
    internal sealed class MaxValueAttributeDrawer : PropertyDrawer
    {
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
