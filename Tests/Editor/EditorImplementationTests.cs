using Jeomseon.Attribute.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

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
                    CreateModification(target, "_first"),
                    CreateModification(target, "_second")
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

        private static UndoPropertyModification CreateModification(
            Object target,
            string propertyPath)
        {
            return new UndoPropertyModification
            {
                currentValue = new PropertyModification
                {
                    target = target,
                    propertyPath = propertyPath
                }
            };
        }
    }

    internal sealed class InspectorChangeTestTarget : ScriptableObject
    {
        [SerializeField] private int _first;
        [SerializeField] private int _second;

        public int InvocationCount { get; private set; }

        [InvokeOnInspectorChange(nameof(_first), nameof(_second))]
        private void OnInspectorChange()
        {
            InvocationCount++;
        }
    }
}
