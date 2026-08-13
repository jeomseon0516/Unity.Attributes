using System;
using System.Linq;
using Jeomseon.Unity.Attributes.Editor;
using Jeomseon.Unity.Attributes.Editor.ConstructorPipelines;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Tests
{
    internal sealed class SerializeReferenceConstructorTests
    {
        [SetUp]
        public void SetUp()
        {
            ConstructorFixture.InvocationCount = 0;
            Undo.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            Undo.ClearAll();
        }

        [Test]
        public void ChooseConstructor_InitializesOptionalDefaults()
        {
            PendingConstructorSelection pending = CreatePending(typeof(ConstructorFixture));
            int constructorIndex = Array.FindIndex(pending.ConstructibleConstructors,
                constructor => constructor.GetParameters().Length == 2);

            ConstructorSelectionService.ChooseConstructor(pending, constructorIndex);

            Assert.That(pending.ParameterValues, Is.EqualTo(new object[] { 7, "기본값" }));
        }

        [Test]
        public void ChooseConstructor_ClearsPreviousPipelineChoice()
        {
            PendingConstructorSelection pending = CreatePending(typeof(ConstructorFixture));
            ConstructorSelectionService.ChooseConstructor(pending, 0);
            pending.ParameterPipelineChoice[0] = 1;

            ConstructorSelectionService.ChooseConstructor(pending, 1);

            Assert.That(pending.ParameterPipelineChoice, Is.Empty);
        }

        [Test]
        public void PipelineRegistry_ReturnsAllMatchingPipelinesAndDisplayNames()
        {
            var candidates = ConstructorParameterPipelineRegistry.GetCandidates(typeof(AmbiguousParameter));

            Assert.That(candidates.Select(candidate => candidate.GetType()),
                Does.Contain(typeof(FirstAmbiguousParameterPipeline)));
            Assert.That(candidates.Select(candidate => candidate.GetType()),
                Does.Contain(typeof(SecondAmbiguousParameterPipeline)));
            Assert.That(candidates.Select(ConstructorParameterPipelineRegistry.GetDisplayName),
                Does.Contain("첫 번째 테스트 파이프라인"));
        }

        [Test]
        public void PipelineRegistry_ProvidesObjectEnumAndVectorDefaults()
        {
            var objectPipeline = ConstructorParameterPipelineRegistry.GetCandidates(typeof(GameObject)).Single();
            var enumPipeline = ConstructorParameterPipelineRegistry.GetCandidates(typeof(TestDirectionMode)).Single();
            var vectorPipeline = ConstructorParameterPipelineRegistry.GetCandidates(typeof(Vector3)).Single();

            Assert.That(objectPipeline.GetDefaultValue(typeof(GameObject)), Is.Null);
            Assert.That(enumPipeline.GetDefaultValue(typeof(TestDirectionMode)), Is.EqualTo(TestDirectionMode.Forward));
            Assert.That(vectorPipeline.GetDefaultValue(typeof(Vector3)), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void TryCreateInstance_InvokesChosenOverloadOnceWithInputValues()
        {
            PendingConstructorSelection pending = CreatePending(typeof(ConstructorFixture));
            int constructorIndex = Array.FindIndex(pending.ConstructibleConstructors,
                constructor => constructor.GetParameters().Length == 2);
            ConstructorSelectionService.ChooseConstructor(pending, constructorIndex);
            pending.ParameterValues = new object[] { 12, "입력값" };

            bool succeeded = ConstructorSelectionService.TryCreateInstance(pending, out object instance, out string error);

            Assert.That(succeeded, Is.True, error);
            Assert.That(instance, Is.TypeOf<ConstructorFixture>());
            Assert.That(((ConstructorFixture)instance).Count, Is.EqualTo(12));
            Assert.That(((ConstructorFixture)instance).Label, Is.EqualTo("입력값"));
            Assert.That(ConstructorFixture.InvocationCount, Is.EqualTo(1));
        }

        [Test]
        public void TryCreateInstance_ReturnsConstructorException()
        {
            PendingConstructorSelection pending = CreatePending(typeof(ThrowingConstructorFixture));
            ConstructorSelectionService.ChooseConstructor(pending, 0);

            bool succeeded = ConstructorSelectionService.TryCreateInstance(pending, out object instance, out string error);

            Assert.That(succeeded, Is.False);
            Assert.That(instance, Is.Null);
            Assert.That(error, Does.Contain(nameof(InvalidOperationException)));
            Assert.That(error, Does.Contain("생성 실패"));
        }

        [Test]
        public void PopupWindow_ClampsLongConstructorFormHeight()
        {
            PendingConstructorSelection pending = CreatePending(typeof(LongConstructorFixture));
            ConstructorSelectionService.ChooseConstructor(pending, 0);
            ConstructorArgumentPopupWindowContent popup = new(pending, () => null);

            Assert.That(popup.GetWindowSize().y, Is.LessThan(ConstructorArgumentFormGUI.GetHeight(pending) + 28f));
        }

        [Test]
        public void ChoosingConstructor_DoesNotChangeSerializedValueBeforeAssignment()
        {
            ConstructorTestTarget target = ScriptableObject.CreateInstance<ConstructorTestTarget>();
            target.Value = new ConstructorFixture(3, "기존값");

            try
            {
                PendingConstructorSelection pending = CreatePending(typeof(ConstructorFixture));
                ConstructorSelectionService.ChooseConstructor(pending, 0);

                Assert.That(target.Value.Count, Is.EqualTo(3));
                Assert.That(target.Value.Label, Is.EqualTo("기존값"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void TryAssign_CreatesIndependentInstanceForEverySelectedTarget()
        {
            ConstructorTestTarget first = ScriptableObject.CreateInstance<ConstructorTestTarget>();
            ConstructorTestTarget second = ScriptableObject.CreateInstance<ConstructorTestTarget>();
            int sequence = 0;

            try
            {
                SerializedObject serializedObject = new(new UnityEngine.Object[] { first, second });

                bool succeeded = ManagedReferenceAssignmentService.TryAssign(
                    serializedObject,
                    nameof(ConstructorTestTarget.Value),
                    () => new ConstructorFixture(++sequence, "새 값"),
                    "Assign Test Value",
                    out string error);

                Assert.That(succeeded, Is.True, error);
                Assert.That(first.Value, Is.Not.SameAs(second.Value));
                Assert.That(first.Value.Count, Is.EqualTo(1));
                Assert.That(second.Value.Count, Is.EqualTo(2));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(first);
                UnityEngine.Object.DestroyImmediate(second);
            }
        }

        [Test]
        public void TryAssign_WhenCreationFails_DoesNotChangeAnyTarget()
        {
            ConstructorTestTarget first = CreateTarget(1);
            ConstructorTestTarget second = CreateTarget(2);
            int invocationCount = 0;

            try
            {
                SerializedObject serializedObject = new(new UnityEngine.Object[] { first, second });

                bool succeeded = ManagedReferenceAssignmentService.TryAssign(
                    serializedObject,
                    nameof(ConstructorTestTarget.Value),
                    () => ++invocationCount == 2
                        ? throw new InvalidOperationException("두 번째 생성 실패")
                        : new ConstructorFixture(10, "미적용"),
                    "Assign Test Value",
                    out string error);

                Assert.That(succeeded, Is.False);
                Assert.That(error, Does.Contain("두 번째 생성 실패"));
                Assert.That(first.Value.Count, Is.EqualTo(1));
                Assert.That(second.Value.Count, Is.EqualTo(2));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(first);
                UnityEngine.Object.DestroyImmediate(second);
            }
        }

        [Test]
        public void TryAssign_RecordsUndo()
        {
            ConstructorTestTarget target = CreateTarget(1);

            try
            {
                SerializedObject serializedObject = new(target);
                ManagedReferenceAssignmentService.TryAssign(
                    serializedObject,
                    nameof(ConstructorTestTarget.Value),
                    () => new ConstructorFixture(9, "새 값"),
                    "Assign Test Value",
                    out string error);
                Assert.That(error, Is.Null);
                Assert.That(target.Value.Count, Is.EqualTo(9));

                Undo.PerformUndo();

                Assert.That(target.Value.Count, Is.EqualTo(1));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private static PendingConstructorSelection CreatePending(Type type)
        {
            return new PendingConstructorSelection(
                type,
                ConstructorParameterPipelineRegistry.GetConstructibleConstructors(type));
        }

        private static ConstructorTestTarget CreateTarget(int count)
        {
            ConstructorTestTarget target = ScriptableObject.CreateInstance<ConstructorTestTarget>();
            target.Value = new ConstructorFixture(count, "기존값");
            return target;
        }
    }

    internal sealed class ConstructorTestTarget : ScriptableObject
    {
        [SerializeReference] public ConstructorFixture Value;
    }

    [Serializable]
    internal sealed class ConstructorFixture
    {
        public static int InvocationCount;
        public int Count;
        public string Label;

        public ConstructorFixture()
        {
            InvocationCount++;
        }

        public ConstructorFixture(int count = 7, string label = "기본값")
        {
            InvocationCount++;
            Count = count;
            Label = label;
        }
    }

    [Serializable]
    internal sealed class ThrowingConstructorFixture
    {
        public ThrowingConstructorFixture()
        {
            throw new InvalidOperationException("생성 실패");
        }
    }

    [Serializable]
    internal sealed class LongConstructorFixture
    {
        public LongConstructorFixture(
            int value01,
            int value02,
            int value03,
            int value04,
            int value05,
            int value06,
            int value07,
            int value08,
            int value09,
            int value10,
            int value11,
            int value12)
        {
        }
    }

    internal readonly struct AmbiguousParameter { }

    internal enum TestDirectionMode
    {
        Forward,
        Backward
    }

    [SerializeReferenceSelectorConstructorPipelineName("첫 번째 테스트 파이프라인")]
    internal sealed class FirstAmbiguousParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(AmbiguousParameter);
        public object GetDefaultValue(Type parameterType) => default(AmbiguousParameter);
        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue) => currentValue;
    }

    internal sealed class SecondAmbiguousParameterPipeline : ISerializeReferenceSelectorConstructorParameterPipeline
    {
        public bool CanHandle(Type parameterType) => parameterType == typeof(AmbiguousParameter);
        public object GetDefaultValue(Type parameterType) => default(AmbiguousParameter);
        public object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue) => currentValue;
    }
}
