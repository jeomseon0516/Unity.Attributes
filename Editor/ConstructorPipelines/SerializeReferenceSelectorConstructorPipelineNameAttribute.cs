#if UNITY_EDITOR
using System;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    /// <summary>
    /// .. 같은 매개변수 타입을 처리하는 파이프라인 구현체가 둘 이상일 때 Inspector에 표시할
    /// 이름을 지정합니다. 지정하지 않으면 구현체 클래스 이름(nameof)을 그대로 사용합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SerializeReferenceSelectorConstructorPipelineNameAttribute : System.Attribute
    {
        public string DisplayName { get; }

        public SerializeReferenceSelectorConstructorPipelineNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
#endif
