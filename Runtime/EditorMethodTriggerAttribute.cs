using System;

namespace Jeomseon.Unity.Attributes
{
    /// <summary>
    /// Editor 이벤트나 Inspector UI를 통해 메서드 호출을 요청하는 Trigger Attribute의 기반입니다.
    /// 트리거 감지와 호출은 Attributes Editor 어셈블리가 담당합니다.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public abstract class EditorMethodTriggerAttribute : System.Attribute
    {
    }
}
