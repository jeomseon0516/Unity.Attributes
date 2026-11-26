#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;

namespace Jeomseon.Unity.Attributes.Editor
{
    using GUI = UnityEngine.GUI;

    [CustomPropertyDrawer(typeof(HierarchyObjectPickerAttribute), true)]
    internal sealed class HierarchyObjectPickerDrawer : PropertyDrawer
    {
        private GUIContent _buttonContent;

        private readonly Dictionary<(EntityId TargetId, string PropertyPath), ComponentDropdown>
            _dropdowns = new();

        // 대상마다 계층이 달라 한 선택을 모든 대상에 적용할 수 없으므로 다중 편집 중에는 Picker를 열지 않습니다.
        internal static bool CanPick(SerializedProperty property) =>
            !property.serializedObject.isEditingMultipleObjects;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.serializedObject.targetObject is not MonoBehaviour monoBehaviour)
                return new HelpBox("HierarchyObjectPicker는 MonoBehaviour에서만 사용할 수 있습니다.", HelpBoxMessageType.Error);

            Type fieldType = GetReferenceType(property);

            if (!typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                return new HelpBox(
                    "HierarchyObjectPicker는 UnityEngine.Object 필드에만 사용할 수 있습니다.",
                    HelpBoxMessageType.Error);
            }

            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return new PropertyField(property);

            ComponentDropdown dropdown = GetOrCreateDropdown(property, monoBehaviour, fieldType);

            // ┌──── label ────┬─ btn ─┬─────── object field ───────┐
            // ObjectField의 정렬 라벨을 그대로 쓰고, 라벨과 입력 영역 사이에 버튼을 넣습니다.
            ObjectField field = new(property.displayName)
            {
                objectType = fieldType,
                bindingPath = property.propertyPath
            };
            field.AddToClassList(BaseField<UnityEngine.Object>.alignedFieldUssClassName);

            Button button = new();
            button.style.width = 18f;
            button.style.marginLeft = 0f;
            button.style.marginRight = 2f;
            button.style.paddingLeft = 0f;
            button.style.paddingRight = 0f;
            button.style.flexShrink = 0f;
            button.Add(new Image
            {
                image = EditorGUIUtility.IconContent("icon dropdown").image,
                scaleMode = ScaleMode.ScaleToFit
            });
            button.clicked += () => dropdown.Show(button.worldBound);
            button.SetEnabled(CanPick(property));
            field.Insert(field.IndexOf(field.labelElement) + 1, button);

            // IMGUI 경로와 같이 선택은 버튼으로만 하고 Object 입력 영역은 비활성화합니다.
            field.Q(className: ObjectField.inputUssClassName)?.SetEnabled(false);
            return field;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _buttonContent ??= new(EditorGUIUtility.IconContent("icon dropdown").image);

            // 이 Attribute는 MonoBehaviour 안에서만 유효
            if (property.serializedObject.targetObject is not MonoBehaviour monoBehaviour)
            {
                EditorGUI.HelpBox(position, "HierarchyObjectPicker는 MonoBehaviour에서만 사용할 수 있습니다.", MessageType.Error);
                return;
            }

            Type fieldType = GetReferenceType(property);

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

            ComponentDropdown dropdown = GetOrCreateDropdown(property, monoBehaviour, fieldType);

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

            using (new EditorGUI.DisabledScope(!CanPick(property)))
            {
                if (GUI.Button(buttonRect, _buttonContent))
                    dropdown.Show(buttonRect);
            }

            GUI.enabled = false;
            EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
            GUI.enabled = true;

            EditorGUI.EndProperty();
        }

        private Type GetReferenceType(SerializedProperty property)
        {
            var fieldType = fieldInfo.FieldType;
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return fieldType;

            if (fieldType.IsArray)
                return fieldType.GetElementType();

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return fieldType.GetGenericArguments()[0];

            return fieldType;
        }

        private ComponentDropdown GetOrCreateDropdown(SerializedProperty property, MonoBehaviour monoBehaviour, Type fieldType)
        {
            UnityEngine.Object targetObject = property.serializedObject.targetObject;
            string propertyPath = property.propertyPath;
            var key = (targetObject.GetEntityId(), propertyPath);

            if (_dropdowns.TryGetValue(key, out ComponentDropdown dropdown))
                return dropdown;

            // 행 ID는 드롭다운마다 순번으로 다시 매겨지므로, 선택·펼침 상태를 다른 드롭다운과 공유하지 않습니다.
            dropdown = new ComponentDropdown(new TreeViewState(), monoBehaviour.gameObject, fieldType, selection =>
            {
                SerializedObject serializedObject = new(targetObject);
                SerializedProperty currentProperty = serializedObject.FindProperty(propertyPath);
                if (currentProperty is null)
                    return;

                Undo.RecordObject(targetObject, "Select hierarchy object");
                currentProperty.objectReferenceValue = selection;
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
            });

            _dropdowns[key] = dropdown;
            return dropdown;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 요소/단일 모두 Unity 기본 높이를 그대로 사용
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
#endif
