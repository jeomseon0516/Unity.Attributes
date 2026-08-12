#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Jeomseon.Attribute;
using Jeomseon.Editor;

namespace Jeomseon.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(SerializeReferenceSelectorAttribute))]
    internal sealed class SerializeReferenceSelectorDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, AdvancedDropdownState> _dropdownStates = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.isArray && property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.LabelField(position, label.text, "SerializeReferenceSelector는 [SerializeReference] 필드에만 사용할 수 있습니다.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            if (property.isArray)
            {
                DrawArray(position, property, label);
            }
            else
            {
                DrawManagedReferenceField(position, property, label);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray && property.propertyType != SerializedPropertyType.ManagedReference)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return property.isArray ? MeasureArrayHeight(property) : MeasureManagedReferenceFieldHeight(property);
        }

        // -------------------- Array container(List<T> 필드에 attribute가 붙는 경우) --------------------
        // PropertyAttribute 기반 커스텀 드로어는 배열/List 필드에 붙으면 배열 전체에 대해 한 번만
        // 호출되고 각 원소에는 다시 호출되지 않습니다(ReadOnlyPropertyDrawer와 동일 전제).
        // 그래서 Foldout·Size·각 원소를 직접 그립니다.

        private static float MeasureArrayHeight(SerializedProperty arrayProperty)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!arrayProperty.isExpanded) return height;

            height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                height += EditorGUIUtility.standardVerticalSpacing +
                    MeasureManagedReferenceFieldHeight(arrayProperty.GetArrayElementAtIndex(i));
            }

            return height;
        }

        private void DrawArray(Rect position, SerializedProperty arrayProperty, GUIContent label)
        {
            Rect foldoutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            arrayProperty.isExpanded = EditorGUI.Foldout(foldoutRect, arrayProperty.isExpanded, label, true);

            if (!arrayProperty.isExpanded) return;

            float y = foldoutRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            Rect sizeRect = new(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            int newSize = Mathf.Max(0, EditorGUI.IntField(sizeRect, "Size", arrayProperty.arraySize));
            if (newSize != arrayProperty.arraySize)
            {
                arrayProperty.arraySize = newSize;
            }
            y = sizeRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
                float elementHeight = MeasureManagedReferenceFieldHeight(element);
                Rect elementRect = new(position.x, y, position.width, elementHeight);
                DrawManagedReferenceField(elementRect, element, new GUIContent($"Element {i}"));
                y = elementRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.indentLevel = indent;
        }

        // -------------------- 단일 [SerializeReference] 필드(배열 원소 포함) --------------------

        private static float MeasureManagedReferenceFieldHeight(SerializedProperty property)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                height += EditorGUIUtility.standardVerticalSpacing + MeasureChildrenHeight(property);
            }

            return height;
        }

        private void DrawManagedReferenceField(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect dropdownRowRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = EditorGUI.PrefixLabel(dropdownRowRect, label);

            Type fieldType = GetManagedReferenceFieldType(property);
            string currentTypeName = GetManagedReferenceTypeDisplayName(property);
            GUIContent buttonContent = new(string.IsNullOrEmpty(currentTypeName) ? "(None)" : currentTypeName);

            if (fieldType != null && EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Keyboard))
            {
                ShowTypeDropdown(buttonRect, property, fieldType);
            }

            if (property.managedReferenceValue is null) return;

            float y = dropdownRowRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            float childrenHeight = position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;
            DrawChildren(new Rect(position.x, y, position.width, childrenHeight), property);
            EditorGUI.indentLevel = indent;
        }

        private void ShowTypeDropdown(Rect activatorRect, SerializedProperty property, Type fieldType)
        {
            string propertyKey = property.propertyPath;
            if (!_dropdownStates.TryGetValue(propertyKey, out AdvancedDropdownState state))
            {
                state = new AdvancedDropdownState();
                _dropdownStates[propertyKey] = state;
            }

            List<Type> concreteTypes = EditorTypeDiscovery.GetConcreteTypesDerivedFrom(fieldType)
                .Where(type => type.GetConstructor(Type.EmptyTypes) is not null)
                .ToList();
            TypeSelectorAdvancedDropdown dropdown = new(state, fieldType, concreteTypes);

            SerializedObject serializedObject = property.serializedObject;
            string path = property.propertyPath;

            dropdown.OnTypeSelected += selectedType =>
            {
                Undo.RecordObjects(serializedObject.targetObjects, "Select Reference Type");
                SerializedProperty targetProperty = serializedObject.FindProperty(path);
                targetProperty.managedReferenceValue = Activator.CreateInstance(selectedType);
                serializedObject.ApplyModifiedProperties();
            };
            dropdown.OnCleared += () =>
            {
                Undo.RecordObjects(serializedObject.targetObjects, "Clear Reference Type");
                SerializedProperty targetProperty = serializedObject.FindProperty(path);
                targetProperty.managedReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
            };

            dropdown.Show(activatorRect);
        }

        // -------------------- 하위 필드 순회 --------------------
        // property 자신을 EditorGUI.PropertyField로 다시 그리면 이 드로어가 재귀 호출되므로,
        // NextVisible/GetEndProperty로 하위 필드만 개별적으로 순회해 그립니다.

        private static float MeasureChildrenHeight(SerializedProperty property)
        {
            float height = 0f;
            ForEachChild(property, child => height += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing);
            return Mathf.Max(0f, height - EditorGUIUtility.standardVerticalSpacing);
        }

        private static void DrawChildren(Rect position, SerializedProperty property)
        {
            float y = position.y;
            ForEachChild(property, child =>
            {
                float height = EditorGUI.GetPropertyHeight(child, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), child, true);
                y += height + EditorGUIUtility.standardVerticalSpacing;
            });
        }

        private static void ForEachChild(SerializedProperty property, Action<SerializedProperty> action)
        {
            SerializedProperty endProperty = property.GetEndProperty();
            SerializedProperty iterator = property.Copy();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;
                action(iterator.Copy());
            }
        }

        // -------------------- managedReferenceFieldTypename → Type 해석 --------------------
        // SerializedProperty에는 Type을 직접 돌려주는 API가 없어(Unity 6000.5 기준),
        // "<AssemblyName> <Namespace.TypeName>" 형식의 문자열을 직접 파싱합니다.

        private static Type GetManagedReferenceFieldType(SerializedProperty property)
        {
            string typename = property.managedReferenceFieldTypename;
            if (string.IsNullOrEmpty(typename)) return null;

            string[] parts = typename.Split(' ');
            if (parts.Length != 2) return null;

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => candidate.GetName().Name == parts[0]);

            return assembly?.GetType(parts[1]);
        }

        private static string GetManagedReferenceTypeDisplayName(SerializedProperty property)
        {
            return property.managedReferenceValue?.GetType().Name ?? string.Empty;
        }
    }
}
#endif
