#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using Jeomseon.Editor.Extensions;

namespace Jeomseon.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(GetOrAddComponentAttribute), false)]
    internal sealed class GetOrAddComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;

            Type requireComponentType = property.GetPropertyType();

            if (requireComponentType is null)
            {
                Debug.LogWarning("Component Type is null");
                return;
            }
            
            if (!typeof(Component).IsAssignableFrom(requireComponentType))
            {
                Debug.LogWarning("Type is not component");
                return;
            }
            
            if (!property.IsNestedAttribute<SerializeField>())
            {
                Debug.LogWarning("check in Contain SerializeField Attribute");
                return;
            }

            if (property.serializedObject.targetObject is not Component component)
            {
                Debug.LogWarning("This Attribute not in Component Context");
                return;
            }

            if (!property.objectReferenceValue)
            {
                if (!component.TryGetComponent(requireComponentType, out Component requireComponent))
                {
                    requireComponent = Undo.AddComponent(component.gameObject, requireComponentType);
                }
                
                if (property.objectReferenceValue != requireComponent)
                {
                    Undo.RecordObject(component, "Assign required component");
                    property.objectReferenceValue = requireComponent;
                    property.serializedObject.ApplyModifiedProperties();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(component);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
#endif
