#if UNITY_EDITOR
namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// InvokeOnInspectorChange Trigger의 현재 매개변수 정책을 제공합니다.
    /// </summary>
    internal sealed class InspectorChangeTriggerHandler
        : ParameterlessEditorMethodTriggerHandler<InvokeOnInspectorChangeAttribute>
    {
    }
}
#endif
