#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;

namespace Jeomseon.Unity.Attributes.Editor
{
    using GUI = UnityEngine.GUI;

    [CustomPropertyDrawer(typeof(HierarchyObjectPickerAttribute), true)]
    internal sealed class HierarchyObjectPickerDrawer : PropertyDrawer
    {
        private readonly TreeViewState _dropdownState = new();
        private GUIContent _buttonContent;

        private readonly Dictionary<(int TargetId, string PropertyPath), ComponentDropdown>
            _dropdowns = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _buttonContent ??= new(EditorGUIUtility.IconContent("icon dropdown").image);

            // 이 Attribute는 MonoBehaviour 안에서만 유효
            if (property.serializedObject.targetObject is not MonoBehaviour monoBehaviour)
            {
                EditorGUI.HelpBox(position, "HierarchyObjectPicker는 MonoBehaviour에서만 사용할 수 있습니다.", MessageType.Error);
                return;
            }

            Type fieldType = fieldInfo.FieldType;

            if (!typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                EditorGUI.HelpBox(position,
                    "HierarchyObjectPicker는 UnityEngine.Object 필드에만 사용할 수 있습니다.",
                    MessageType.Error);
                return;
            }

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            UnityEngine.Object targetObject = property.serializedObject.targetObject;
            string propertyPath = property.propertyPath;
            (int TargetId, string PropertyPath) key =
                (targetObject.GetEntityId().GetHashCode(), propertyPath);

            if (!_dropdowns.TryGetValue(key, out ComponentDropdown dropdown))
            {
                dropdown = new ComponentDropdown(_dropdownState, monoBehaviour.gameObject, fieldType, go =>
                {
                    SerializedObject serializedObject = new(targetObject);
                    SerializedProperty currentProperty = serializedObject.FindProperty(propertyPath);
                    if (currentProperty is null)
                        return;

                    Undo.RecordObject(targetObject, "Select hierarchy object");
                    if (fieldType == typeof(GameObject))
                    {
                        currentProperty.objectReferenceValue = go;
                    }
                    else
                    {
                        currentProperty.objectReferenceValue = go.GetComponent(fieldType);
                    }

                    serializedObject.ApplyModifiedProperties();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
                });

                _dropdowns[key] = dropdown;
            }

            EditorGUI.BeginProperty(position, label, property);

            // ┌──── label ────┬─ btn ─┬─────── object field ───────┐
            Rect labelRect  = new(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Rect buttonRect = new(labelRect.xMax, position.y, 18f, position.height);
            Rect fieldRect  = new(
                buttonRect.xMax + 2f,
                position.y,
                position.width - (buttonRect.xMax - position.x),
                position.height);

            EditorGUI.LabelField(labelRect, label);

            if (GUI.Button(buttonRect, _buttonContent))
            {
                dropdown.Show(buttonRect);
            }

            GUI.enabled = false;
            EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
            GUI.enabled = true;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 요소/단일 모두 Unity 기본 높이를 그대로 사용
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
#endif
