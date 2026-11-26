using System.Collections.Generic;
using System.Reflection;
using Jeomseon.Unity.Attributes.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Attribute.Tests
{
    internal sealed class HierarchyObjectPickerTests
    {
        [TestCase("single", -1)]
        [TestCase("slots", 1)]
        [TestCase("list", 1)]
        public void Picker_AssignmentCanBeUndoneAndSaved(string fieldName, int slot)
        {
            var root = new GameObject("Root");
            var child = new GameObject("Child");
            child.transform.SetParent(root.transform);
            var owner = root.AddComponent<HierarchyObjectPickerTestComponent>();
            var selected = child.AddComponent<HierarchyObjectPickerTestComponent>();
            var prefabPath = AssetDatabase.GenerateUniqueAssetPath("Assets/HierarchyObjectPickerTest.prefab");

            try
            {
                using var serialized = new SerializedObject(owner);
                using var field = serialized.FindProperty(fieldName);
                using var property = slot < 0 ? field.Copy() : field.GetArrayElementAtIndex(slot);
                var drawer = new HierarchyObjectPickerDrawer();
                const BindingFlags privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
                typeof(PropertyDrawer).GetField("m_FieldInfo", privateInstance).SetValue(
                    drawer, typeof(HierarchyObjectPickerTestComponent).GetField(fieldName, privateInstance));

                var element = drawer.CreatePropertyGUI(property);
                Assert.That(element, Is.TypeOf<ObjectField>());
                Assert.That(((ObjectField)element).objectType, Is.EqualTo(typeof(HierarchyObjectPickerTestComponent)));
                Assert.That(element.Q<Button>(), Is.Not.Null);

                var dropdowns = (Dictionary<(int, string), ComponentDropdown>)
                    typeof(HierarchyObjectPickerDrawer).GetField("_dropdowns", privateInstance).GetValue(drawer);
                var dropdown = dropdowns[(owner.GetEntityId().GetHashCode(), property.propertyPath)];
                dropdown.Reload();
                Undo.IncrementCurrentGroup();
                dropdown.Choose(child.GetEntityId().GetHashCode());
                Undo.FlushUndoRecordObjects();

                AssertReference(selected);
                Undo.PerformUndo();
                AssertReference(null);
                Undo.PerformRedo();
                AssertReference(selected);

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                AssetDatabase.SaveAssets();
                var saved = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                using var savedOwner = new SerializedObject(saved.GetComponent<HierarchyObjectPickerTestComponent>());
                using var savedProperty = savedOwner.FindProperty(property.propertyPath);
                Assert.That(savedProperty.objectReferenceValue,
                    Is.EqualTo(saved.transform.GetChild(0).GetComponent<HierarchyObjectPickerTestComponent>()));

                void AssertReference(HierarchyObjectPickerTestComponent expected)
                {
                    serialized.Update();
                    Assert.That(property.objectReferenceValue, Is.EqualTo(expected));
                    if (slot >= 0)
                    {
                        using var firstSlot = field.GetArrayElementAtIndex(0);
                        Assert.That(firstSlot.objectReferenceValue, Is.Null);
                    }
                }
            }
            finally
            {
                Undo.ClearUndo(owner);
                Object.DestroyImmediate(root);
                AssetDatabase.DeleteAsset(prefabPath);
            }
        }
    }
}
