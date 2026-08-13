using System;
using System.Diagnostics;

namespace Jeomseon.Unity.Attributes
{
    /// <summary>
    /// Inspector에 버튼을 표시하고 클릭했을 때 메서드 호출을 요청합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false), Conditional("UNITY_EDITOR")]
    public sealed class InspectorButtonAttribute : EditorMethodTriggerAttribute
    {
        public string Label { get; }

        public InspectorButtonAttribute(string label)
        {
            Label = label ?? string.Empty;
        }
    }
}
