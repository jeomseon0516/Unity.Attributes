#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jeomseon.Unity.Attributes.Editor
{
    /// <summary>
    /// .. SerializeReferenceSelector에서 매개변수 생성자를 가진 타입을 선택했을 때, 실제
    /// managedReferenceValue 대입 전까지 입력 중인 상태를 보관합니다. SerializedProperty를 전혀
    /// 건드리지 않는 순수 UI 임시 상태라 Undo 대상이 아닙니다.
    /// </summary>
    internal sealed class PendingConstructorSelection
    {
        public Type SelectedType { get; }
        public ConstructorInfo[] ConstructibleConstructors { get; }
        public int? ChosenConstructorIndex { get; set; }
        public object[] ParameterValues { get; set; }
        public Dictionary<int, int> ParameterPipelineChoice { get; } = new();
        public string ErrorMessage { get; set; }

        public PendingConstructorSelection(Type selectedType, ConstructorInfo[] constructibleConstructors)
        {
            SelectedType = selectedType;
            ConstructibleConstructors = constructibleConstructors;
        }

        public ConstructorInfo ChosenConstructor => ChosenConstructorIndex.HasValue
            ? ConstructibleConstructors[ChosenConstructorIndex.Value]
            : null;
    }
}
#endif
