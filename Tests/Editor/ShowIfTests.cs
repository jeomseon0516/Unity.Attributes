using System;
using Jeomseon.Unity.Attributes.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Jeomseon.Attribute.Tests
{
    public sealed class ShowIfTests
    {
        private enum Mode
        {
            Default,
            Separate = 7
        }

        [Serializable]
        private sealed class ConditionGroup
        {
            [field: SerializeField] public Mode DisplayMode { get; set; } = Mode.Separate;
            public bool visible = true;
            public int count = 2;
            public string details = "Details";
        }

        private sealed class Fixture : ScriptableObject
        {
            [SerializeField] private ConditionGroup group = new();
        }

        [TestCase("visible", 1, true)]
        [TestCase("visible", 0, false)]
        [TestCase("count", 2, true)]
        [TestCase("count", 0, false)]
        [TestCase("DisplayMode", 7, true)]
        [TestCase("DisplayMode", 1, false)]
        [TestCase("details", 0, true)]
        [TestCase("missing", 0, true)]
        public void EvaluatesSiblingConditions(string memberName, int expected, bool visible)
        {
            var fixture = ScriptableObject.CreateInstance<Fixture>();
            try
            {
                using var serialized = new SerializedObject(fixture);
                var property = serialized.FindProperty("group.details");
                var condition = ShowIfDrawer.FindCondition(property, memberName);
                Assert.That(ShowIfDrawer.IsVisible(condition, expected), Is.EqualTo(visible));
            }
            finally
            {
                Object.DestroyImmediate(fixture);
            }
        }

        [Test]
        public void MixedSelectionKeepsTheFieldVisible()
        {
            var first = ScriptableObject.CreateInstance<Fixture>();
            var second = ScriptableObject.CreateInstance<Fixture>();
            try
            {
                using var single = new SerializedObject(second);
                single.FindProperty("group.visible").boolValue = false;
                single.ApplyModifiedPropertiesWithoutUndo();

                using var multi = new SerializedObject(new Object[] { first, second });
                var condition = ShowIfDrawer.FindCondition(multi.FindProperty("group.details"), "visible");

                Assert.That(condition.hasMultipleDifferentValues, Is.True);
                Assert.That(ShowIfDrawer.IsVisible(condition, 0), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
            }
        }
    }
}
