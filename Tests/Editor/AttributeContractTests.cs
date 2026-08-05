using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Jeomseon.Attribute.Tests
{
    internal sealed class AttributeContractTests
    {
        [TestCase(typeof(InfoBoxAttribute), AttributeTargets.Field)]
        [TestCase(typeof(MaxValueAttribute), AttributeTargets.Field)]
        [TestCase(typeof(ReadOnlyAttribute), AttributeTargets.Field)]
        [TestCase(typeof(SpritePreviewAttribute), AttributeTargets.Field)]
        [TestCase(typeof(Vector2SliderAttribute), AttributeTargets.Field)]
        [TestCase(typeof(GetOrAddComponentAttribute), AttributeTargets.Field)]
        [TestCase(typeof(HierarchyObjectPickerAttribute), AttributeTargets.Field)]
        [TestCase(typeof(InspectorButtonAttribute), AttributeTargets.Method)]
        [TestCase(typeof(InvokeOnInspectorChangeAttribute), AttributeTargets.Method)]
        public void AttributeUsage_IsSingleAndInherited(Type type, AttributeTargets targets)
        {
            AttributeUsageAttribute usage = type
                .GetCustomAttributes(typeof(AttributeUsageAttribute), true)
                .Cast<AttributeUsageAttribute>()
                .Single();

            Assert.That(usage.ValidOn, Is.EqualTo(targets));
            Assert.That(usage.AllowMultiple, Is.False);
            Assert.That(usage.Inherited, Is.True);
        }

        [TestCase(typeof(InfoBoxAttribute))]
        [TestCase(typeof(MaxValueAttribute))]
        [TestCase(typeof(ReadOnlyAttribute))]
        [TestCase(typeof(SpritePreviewAttribute))]
        [TestCase(typeof(Vector2SliderAttribute))]
        [TestCase(typeof(GetOrAddComponentAttribute))]
        [TestCase(typeof(HierarchyObjectPickerAttribute))]
        [TestCase(typeof(InspectorButtonAttribute))]
        [TestCase(typeof(InvokeOnInspectorChangeAttribute))]
        public void EditorOnlyAttribute_HasConditionalContract(Type type)
        {
            ConditionalAttribute conditional = type
                .GetCustomAttributes(typeof(ConditionalAttribute), true)
                .Cast<ConditionalAttribute>()
                .Single();

            Assert.That(conditional.ConditionString, Is.EqualTo("UNITY_EDITOR"));
        }

        [Test]
        public void Vector2Slider_NormalizesReversedBounds()
        {
            Vector2SliderAttribute attribute = new(10f, -2f);

            Assert.That(attribute.Min, Is.EqualTo(-2f));
            Assert.That(attribute.Max, Is.EqualTo(10f));
        }

        [Test]
        public void SpritePreview_ClampsNegativeSize()
        {
            SpritePreviewAttribute attribute = new(-1f);

            Assert.That(attribute.Size, Is.Zero);
        }

        [Test]
        public void InfoBox_NormalizesNullMessage()
        {
            InfoBoxAttribute attribute = new(null);

            Assert.That(attribute.Message, Is.Empty);
            Assert.That(attribute.Type, Is.EqualTo(InfoBoxType.Info));
        }

        [Test]
        public void InspectorButton_NormalizesNullLabel()
        {
            InspectorButtonAttribute attribute = new(null);

            Assert.That(attribute.Label, Is.Empty);
        }
    }
}
