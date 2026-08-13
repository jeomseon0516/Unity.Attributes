#if UNITY_EDITOR
namespace Jeomseon.Unity.Attributes.Editor
{
    /// <summary>
    /// InspectorButton Trigger의 현재 매개변수 정책을 제공합니다.
    /// </summary>
    internal sealed class InspectorButtonTriggerHandler
        : ParameterlessEditorMethodTriggerHandler<InspectorButtonAttribute>
    {
    }
}
#endif
