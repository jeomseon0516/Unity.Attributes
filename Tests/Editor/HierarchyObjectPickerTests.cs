using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
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
        [Test]
        public void Picker_GameObjectRemainsTheSelection()
        {
            var root = new GameObject("Root");
            try
            {
                Object selected = null;
                var dropdown = new ComponentDropdown(new TreeViewState(), root, typeof(GameObject),
                    value => selected = value);
                dropdown.Reload();
                var row = dropdown.CreateElementItems().Single();
                Assert.That(row.data.displayName, Is.EqualTo("Root (GameObject)"));
                dropdown.Choose(row.id);
                Assert.That(selected, Is.SameAs(root));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [TestCase("single", -1, false)]
        [TestCase("slots", 1, false)]
        [TestCase("list", 1, false)]
        [TestCase("single", -1, true)]
        [TestCase("slots", 1, true)]
        [TestCase("list", 1, true)]
        public void Picker_AssignmentCanBeUndoneAndSaved(string fieldName, int slot, bool multiple)
        {
            var root = new GameObject("Root");
            var child = new GameObject("Child");
            child.transform.SetParent(root.transform);
            var owner = root.AddComponent<HierarchyObjectPickerTestComponent>();
            HierarchyObjectPickerTestComponent[] candidates = multiple
                ? new HierarchyObjectPickerTestComponent[]
                {
                    child.AddComponent<FirstPickerComponent>(),
                    child.AddComponent<SecondPickerComponent>(),
                    child.AddComponent<FirstPickerComponent>()
                }
                : new[] { child.AddComponent<HierarchyObjectPickerTestComponent>() };
            var selected = candidates[^1];
            var nested = new GameObject("Nested");
            nested.transform.SetParent(child.transform);
            if (multiple)
                nested.AddComponent<SecondPickerComponent>();
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
                var childId = child.GetEntityId().GetHashCode();
                HierarchyObjectPickerTestComponent previous = null;
                if (multiple)
                {
                    var objectRow = dropdown.CreateElementItems().Single().children.Single();
                    Assert.That(objectRow.id, Is.EqualTo(childId));
                    Assert.That(objectRow.data.depth, Is.EqualTo(1));
                    var group = objectRow.children.Single(row => row.data.displayName == "Components");
                    var nestedRow = objectRow.children.Single(row => row.id == nested.GetEntityId().GetHashCode());
                    Assert.That(objectRow.children.Select(row => row.id), Is.EqualTo(new[] { group.id, nestedRow.id }));
                    dropdown.ExpandAll();
                    var imguiRows = dropdown.GetRows().Select(row => row.id).ToList();
                    Assert.That(imguiRows.IndexOf(group.id), Is.LessThan(imguiRows.IndexOf(nestedRow.id)));
                    Assert.That(imguiRows.IndexOf(candidates[^1].GetEntityId().GetHashCode()),
                        Is.LessThan(imguiRows.IndexOf(nestedRow.id)));
                    Assert.That(dropdown.IsGroup(group.data), Is.True);
                    Assert.That(dropdown.IsGroup(nestedRow.data), Is.False);
                    Assert.That(dropdown.HasSeparatorBefore(nestedRow.data), Is.True);
                    Assert.That(group.children.Any(row => dropdown.HasSeparatorBefore(row.data)), Is.False);
                    Assert.That(group.data.depth, Is.EqualTo(2));
                    Assert.That(nestedRow.data.depth, Is.EqualTo(group.data.depth));
                    Assert.That(group.children.Select(row => row.data.displayName), Is.EqualTo(new[]
                    {
                        "Child (FirstPickerComponent) #1",
                        "Child (SecondPickerComponent)",
                        "Child (FirstPickerComponent) #2"
                    }));
                    Assert.That(group.children.Select(row => row.data.depth), Is.All.EqualTo(3));
                    dropdown.Choose(childId);
                    dropdown.Choose(group.id);
                    AssertReference(null);

                    var unfiltered = dropdown.searchString;
                    dropdown.searchString = nameof(FirstPickerComponent);
                    Assert.That(dropdown.CreateElementItems().Select(row => row.id), Is.EqualTo(new[]
                    {
                        candidates[0].GetEntityId().GetHashCode(),
                        candidates[2].GetEntityId().GetHashCode()
                    }));
                    dropdown.searchString = nameof(SecondPickerComponent);
                    Assert.That(dropdown.CreateElementItems().Select(row => row.id), Is.EqualTo(new[]
                    {
                        candidates[1].GetEntityId().GetHashCode(),
                        nested.GetEntityId().GetHashCode()
                    }));
                    Assert.That(dropdown.HasSeparatorBefore(nestedRow.data), Is.False);
                    dropdown.searchString = "Picker";
                    dropdown.Reload();
                    Assert.That(dropdown.GetRows().Select(row => row.id),
                        Is.EqualTo(dropdown.CreateElementItems().Select(row => row.id)));
                    dropdown.searchString = unfiltered;
                    foreach (var candidate in candidates.Take(candidates.Length - 1))
                    {
                        Choose(candidate.GetEntityId().GetHashCode());
                        AssertReference(candidate);
                        previous = candidate;
                    }
                }

                Choose(multiple ? selected.GetEntityId().GetHashCode() : childId);
                AssertReference(selected);
                Undo.PerformUndo();
                AssertReference(previous);
                Undo.PerformRedo();
                AssertReference(selected);

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                AssetDatabase.SaveAssets();
                var saved = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                using var savedOwner = new SerializedObject(saved.GetComponent<HierarchyObjectPickerTestComponent>());
                using var savedProperty = savedOwner.FindProperty(property.propertyPath);
                Assert.That(savedProperty.objectReferenceValue,
                    Is.EqualTo(saved.transform.GetChild(0).GetComponents<HierarchyObjectPickerTestComponent>()[^1]));

                void Choose(int id)
                {
                    Undo.IncrementCurrentGroup();
                    dropdown.Choose(id);
                    Undo.FlushUndoRecordObjects();
                }

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
