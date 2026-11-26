using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<int>;
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

                var dropdowns = (Dictionary<(EntityId, string), ComponentDropdown>)
                    typeof(HierarchyObjectPickerDrawer).GetField("_dropdowns", privateInstance).GetValue(drawer);
                var dropdown = dropdowns[(owner.GetEntityId(), property.propertyPath)];
                dropdown.Reload();
                var ids = RowIds(dropdown);
                var childId = ids[multiple ? "Child (GameObject)" : "Child (HierarchyObjectPickerTestComponent)"];
                var candidateIds = multiple
                    ? new[]
                    {
                        ids["Child (FirstPickerComponent) #1"],
                        ids["Child (SecondPickerComponent)"],
                        ids["Child (FirstPickerComponent) #2"]
                    }
                    : new[] { childId };
                HierarchyObjectPickerTestComponent previous = null;
                if (multiple)
                {
                    var nestedId = ids["Nested (SecondPickerComponent)"];
                    var objectRow = dropdown.CreateElementItems().Single().children.Single();
                    Assert.That(objectRow.id, Is.EqualTo(childId));
                    Assert.That(objectRow.data.depth, Is.EqualTo(1));
                    var group = objectRow.children.Single(row => row.data.displayName == "Components");
                    var nestedRow = objectRow.children.Single(row => row.id == nestedId);
                    Assert.That(objectRow.children.Select(row => row.id), Is.EqualTo(new[] { group.id, nestedRow.id }));
                    dropdown.ExpandAll();
                    var imguiRows = dropdown.GetRows().Select(row => row.id).ToList();
                    Assert.That(imguiRows.IndexOf(group.id), Is.LessThan(imguiRows.IndexOf(nestedRow.id)));
                    Assert.That(imguiRows.IndexOf(candidateIds[^1]),
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
                        candidateIds[0],
                        candidateIds[2]
                    }));
                    dropdown.searchString = nameof(SecondPickerComponent);
                    Assert.That(dropdown.CreateElementItems().Select(row => row.id), Is.EqualTo(new[]
                    {
                        candidateIds[1],
                        nestedId
                    }));
                    Assert.That(dropdown.HasSeparatorBefore(nestedRow.data), Is.False);
                    dropdown.searchString = "Picker";
                    dropdown.Reload();
                    Assert.That(dropdown.GetRows().Select(row => row.id),
                        Is.EqualTo(dropdown.CreateElementItems().Select(row => row.id)));
                    dropdown.searchString = unfiltered;
                    for (var index = 0; index < candidates.Length - 1; index++)
                    {
                        Choose(candidateIds[index]);
                        AssertReference(candidates[index]);
                        previous = candidates[index];
                    }
                }

                Choose(candidateIds[^1]);
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

        [Test]
        public void Picker_RowIdsAreUniqueAndMapToTheirOwnInstances()
        {
            var root = new GameObject("Root");
            try
            {
                var expected = new Dictionary<string, Object>
                {
                    ["Root (HierarchyObjectPickerTestComponent)"] = root.AddComponent<HierarchyObjectPickerTestComponent>()
                };
                var child = new GameObject("Child");
                child.transform.SetParent(root.transform);
                for (var ordinal = 1; ordinal <= 3; ordinal++)
                    expected[$"Child (FirstPickerComponent) #{ordinal}"] = child.AddComponent<FirstPickerComponent>();
                expected["Child (SecondPickerComponent)"] = child.AddComponent<SecondPickerComponent>();
                for (var index = 0; index < 8; index++)
                {
                    var leaf = new GameObject($"Leaf {index}");
                    leaf.transform.SetParent(index % 2 == 0 ? child.transform : root.transform);
                    expected[$"Leaf {index} (SecondPickerComponent)"] = leaf.AddComponent<SecondPickerComponent>();
                }

                Object selected = null;
                var dropdown = new ComponentDropdown(new TreeViewState(), root,
                    typeof(HierarchyObjectPickerTestComponent), value => selected = value);
                dropdown.Reload();

                var rows = Flatten(dropdown.CreateElementItems()).ToList();
                Assert.That(rows.Select(row => row.id), Is.Unique);

                var group = rows.Single(row => dropdown.IsGroup(row.data));
                Assert.That(group.children.Select(row => row.data.displayName), Is.EqualTo(new[]
                {
                    "Child (FirstPickerComponent) #1",
                    "Child (FirstPickerComponent) #2",
                    "Child (FirstPickerComponent) #3",
                    "Child (SecondPickerComponent)"
                }));

                foreach (var row in rows)
                {
                    selected = null;
                    dropdown.Choose(row.id);
                    Assert.That(selected, Is.SameAs(expected.GetValueOrDefault(row.data.displayName)),
                        row.data.displayName);
                }

                Assert.That(rows.Count(row => expected.ContainsKey(row.data.displayName)), Is.EqualTo(expected.Count));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Picker_IsDisabledWhileEditingMultipleObjects()
        {
            var first = new GameObject("First").AddComponent<HierarchyObjectPickerTestComponent>();
            var second = new GameObject("Second").AddComponent<HierarchyObjectPickerTestComponent>();
            try
            {
                using (var multiple = new SerializedObject(new Object[] { first, second }))
                using (var multipleProperty = multiple.FindProperty("single"))
                {
                    var multipleDrawer = CreateDrawer("single");
                    var multipleField = multipleDrawer.CreatePropertyGUI(multipleProperty);
                    Assert.That(multipleField, Is.TypeOf<ObjectField>());
                    Assert.That(multipleField.Q<Button>().enabledSelf, Is.False);
                    Assert.That(HierarchyObjectPickerDrawer.CanPick(multipleProperty), Is.False);
                }

                using var single = new SerializedObject(first);
                using var singleProperty = single.FindProperty("single");
                var drawer = CreateDrawer("single");
                var field = drawer.CreatePropertyGUI(singleProperty);
                Assert.That(field.Q<Button>().enabledSelf, Is.True);
                Assert.That(HierarchyObjectPickerDrawer.CanPick(singleProperty), Is.True);

                const BindingFlags privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
                var dropdowns = (Dictionary<(EntityId, string), ComponentDropdown>)
                    typeof(HierarchyObjectPickerDrawer).GetField("_dropdowns", privateInstance).GetValue(drawer);
                var dropdown = dropdowns[(first.GetEntityId(), singleProperty.propertyPath)];
                dropdown.Reload();
                dropdown.Choose(RowIds(dropdown)["First (HierarchyObjectPickerTestComponent)"]);
                single.Update();
                Assert.That(singleProperty.objectReferenceValue, Is.SameAs(first));
            }
            finally
            {
                Undo.ClearUndo(first);
                Object.DestroyImmediate(first.gameObject);
                Object.DestroyImmediate(second.gameObject);
            }
        }

        private static HierarchyObjectPickerDrawer CreateDrawer(string fieldName)
        {
            const BindingFlags privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            var drawer = new HierarchyObjectPickerDrawer();
            typeof(PropertyDrawer).GetField("m_FieldInfo", privateInstance).SetValue(
                drawer, typeof(HierarchyObjectPickerTestComponent).GetField(fieldName, privateInstance));
            return drawer;
        }

        private static Dictionary<string, int> RowIds(ComponentDropdown dropdown) =>
            Flatten(dropdown.CreateElementItems()).ToDictionary(row => row.data.displayName, row => row.id);

        private static IEnumerable<TreeViewItemData<TreeViewItem>> Flatten(
            IEnumerable<TreeViewItemData<TreeViewItem>> rows)
        {
            foreach (var row in rows)
            {
                yield return row;
                if (row.children is null)
                    continue;

                foreach (var child in Flatten(row.children))
                    yield return child;
            }
        }
    }
}
