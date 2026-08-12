using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Jeomseon.Attribute.Tests
{
    // Play Mode 진입/종료 후 [SerializeReference] 내부 값이 Edit Mode 상태로 복원되는지 확인합니다.
    // 이 assembly는 EditMode 전용이지만, EnterPlayMode/ExitPlayMode yield 명령으로 실제 Play Mode
    // 전환(기본 설정이면 Domain Reload 포함)을 코드로 재현할 수 있습니다. 두 케이스를 비교해
    // [SerializeReferenceSelector] 유무가 결과에 영향을 주는지(B) 아니면 Unity의 순수
    // [SerializeReference] 자체 동작인지(A)를 가릅니다.
    //
    // Domain Reload가 켜져 있으면 EnterPlayMode 이후 C# 힙이 재생성되므로, yield 경계를 넘어
    // GameObject/Component 참조를 직접 들고 있지 않고 매번 이름으로 다시 찾습니다.
    [TestFixture]
    internal sealed class ManagedReferencePlayModeRestoreTests
    {
        private const string PlainObjectName = "__PlainManagedReferenceTestTarget";
        private const string SelectorObjectName = "__SelectorManagedReferenceTestTarget";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            DestroyAllWithName(PlainObjectName);
            DestroyAllWithName(SelectorObjectName);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            DestroyAllWithName(PlainObjectName);
            DestroyAllWithName(SelectorObjectName);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlainSerializeReference_ValueChangedDuringPlayMode_RevertsAfterExitingPlayMode()
        {
            GameObject go = new(PlainObjectName);
            PlainManagedReferenceTestComponent component = go.AddComponent<PlainManagedReferenceTestComponent>();
            component.Plain = new ManagedReferenceTestPayload { Value = 1 };

            SerializedObject serializedObject = new(component);
            SerializedProperty valueProperty = serializedObject.FindProperty(nameof(PlainManagedReferenceTestComponent.Plain))
                .FindPropertyRelative(nameof(ManagedReferenceTestPayload.Value));
            Assert.That(valueProperty.intValue, Is.EqualTo(1));

            yield return new EnterPlayMode();

            PlainManagedReferenceTestComponent runtimeComponent = FindRequiredComponent<PlainManagedReferenceTestComponent>(PlainObjectName);
            SerializedObject runtimeSerializedObject = new(runtimeComponent);
            SerializedProperty runtimeValueProperty = runtimeSerializedObject.FindProperty(nameof(PlainManagedReferenceTestComponent.Plain))
                .FindPropertyRelative(nameof(ManagedReferenceTestPayload.Value));
            runtimeValueProperty.intValue = 100;
            runtimeSerializedObject.ApplyModifiedProperties();

            yield return new ExitPlayMode();

            PlainManagedReferenceTestComponent afterComponent = FindRequiredComponent<PlainManagedReferenceTestComponent>(PlainObjectName);

            // 여기서 100이 아니라 1이 나와야 정상적인 Play Mode 복원입니다. 순수
            // [SerializeReference]만으로도 100이 남는다면 Category A(Unity 자체 동작/한계)입니다.
            Assert.That(afterComponent.Plain.Value, Is.EqualTo(1),
                "순수 [SerializeReference]만 사용해도 Play Mode 값이 복원되지 않으면 이 프로젝트의 " +
                "SerializeReferenceSelector 구현과 무관한 Unity 자체 동작(Category A)입니다.");
        }

        [UnityTest]
        public IEnumerator SelectorSerializeReference_ValueChangedDuringPlayMode_RevertsAfterExitingPlayMode()
        {
            GameObject go = new(SelectorObjectName);
            SelectorManagedReferenceTestComponent component = go.AddComponent<SelectorManagedReferenceTestComponent>();
            component.WithSelector = new ManagedReferenceTestPayload { Value = 1 };

            yield return new EnterPlayMode();

            SelectorManagedReferenceTestComponent runtimeComponent = FindRequiredComponent<SelectorManagedReferenceTestComponent>(SelectorObjectName);
            SerializedObject runtimeSerializedObject = new(runtimeComponent);
            SerializedProperty runtimeValueProperty = runtimeSerializedObject.FindProperty(nameof(SelectorManagedReferenceTestComponent.WithSelector))
                .FindPropertyRelative(nameof(ManagedReferenceTestPayload.Value));
            runtimeValueProperty.intValue = 100;
            runtimeSerializedObject.ApplyModifiedProperties();

            yield return new ExitPlayMode();

            SelectorManagedReferenceTestComponent afterComponent = FindRequiredComponent<SelectorManagedReferenceTestComponent>(SelectorObjectName);

            // 이 테스트가 실패하는데 위 Plain 테스트는 통과한다면 Category B(SerializeReferenceSelector
            // 구현 문제)로 확정할 수 있습니다. 둘 다 같은 결과라면 Category A입니다.
            Assert.That(afterComponent.WithSelector.Value, Is.EqualTo(1));
        }

        private static T FindRequiredComponent<T>(string objectName) where T : Component
        {
            GameObject target = GameObject.Find(objectName);
            Assert.That(target, Is.Not.Null, $"'{objectName}' GameObject를 찾지 못했습니다.");

            T component = target.GetComponent<T>();
            Assert.That(component, Is.Not.Null,
                $"'{objectName}' GameObject에서 {typeof(T).Name} 컴포넌트를 찾지 못했습니다. " +
                "Play Mode 전환 중 스크립트 참조가 유실되었거나 동명 오브젝트가 남아 있을 수 있습니다.");
            return component;
        }

        private static void DestroyAllWithName(string objectName)
        {
            GameObject[] targets = UnityEngine.Object.FindObjectsByType<GameObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (GameObject target in targets)
            {
                if (target.name == objectName)
                {
                    UnityEngine.Object.DestroyImmediate(target);
                }
            }
        }
    }
}
