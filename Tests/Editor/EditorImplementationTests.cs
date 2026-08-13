using Jeomseon.Unity.Attributes;
using Jeomseon.Unity.Attributes.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Attribute.Tests
{
    internal sealed class EditorImplementationTests
    {
        [TestCase(5f, 3f, 3f)]
        [TestCase(-5f, 3f, -5f)]
        public void MaxValue_ClampsFloat(float value, float max, float expected)
        {
            Assert.That(MaxValueAttributeDrawer.Clamp(value, max), Is.EqualTo(expected));
        }

        [TestCase(5, 3.9f, 3)]
        [TestCase(-5, 3.9f, -5)]
        public void MaxValue_ClampsInteger(int value, float max, int expected)
        {
            Assert.That(MaxValueAttributeDrawer.Clamp(value, max), Is.EqualTo(expected));
        }

        [Test]
        public void InvokeOnInspectorChange_DeduplicatesMethodPerModificationBatch()
        {
            InspectorChangeTestTarget target =
                ScriptableObject.CreateInstance<InspectorChangeTestTarget>();

            try
            {
                UndoPropertyModification[] modifications =
                {
                    CreateModification(target, "first"),
                    CreateModification(target, "second")
                };

                InvokeOnInspectorChangeProcessor.OnPostprocessModifications(modifications);
                InvokeOnInspectorChangeProcessor.InvokePendingMethods();

                Assert.That(target.InvocationCount, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void InvokeOnInspectorChange_IgnoresUnchangedModification()
        {
            InspectorChangeTestTarget target =
                ScriptableObject.CreateInstance<InspectorChangeTestTarget>();

            try
            {
                UndoPropertyModification[] modifications =
                {
                    CreateModification(target, "first", "1", "1")
                };

                InvokeOnInspectorChangeProcessor.OnPostprocessModifications(modifications);
                InvokeOnInspectorChangeProcessor.InvokePendingMethods();

                Assert.That(target.InvocationCount, Is.EqualTo(0));
            }
            finally
            {
                Object.DestroyImmediate(target);
            }
        }

        private static UndoPropertyModification CreateModification(
            Object target,
            string propertyPath,
            string previousValue = null,
            string currentValue = null)
        {
            return new UndoPropertyModification
            {
                previousValue = previousValue == null
                    ? null
                    : new PropertyModification
                    {
                        target = target,
                        propertyPath = propertyPath,
                        value = previousValue
                    },
                currentValue = new PropertyModification
                {
                    target = target,
                    propertyPath = propertyPath,
                    value = currentValue
                }
            };
        }
    }

    internal sealed class InspectorChangeTestTarget : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("_first")] private int first;
        [SerializeField, FormerlySerializedAs("_second")] private int second;

        public int InvocationCount { get; private set; }

        [InvokeOnInspectorChange(nameof(first), nameof(second))]
        private void OnInspectorChange()
        {
            InvocationCount++;
        }
    }
}
