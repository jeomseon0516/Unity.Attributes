using System;
using System.Diagnostics;

namespace Jeomseon.Unity.Attributes
{
    /// <summary>
    /// Inspector 또는 Editor 도구에서 지정한 직렬화 필드가 Undo를 통해 변경되면 이 메서드를 호출합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false), Conditional("UNITY_EDITOR")]
    public sealed class InvokeOnInspectorChangeAttribute : EditorMethodTriggerAttribute
    {
        public string[] FieldNames { get; }

        public InvokeOnInspectorChangeAttribute(params string[] fieldNames)
        {
            FieldNames = fieldNames ?? Array.Empty<string>();
        }
    }
}
