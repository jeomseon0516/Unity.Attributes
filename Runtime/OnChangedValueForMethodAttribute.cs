using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Attribute
{
    /// <summary>
    /// 에디터에서 지정한 직렬화 필드의 값이 변경되었을 때 이 메서드를 호출합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true), Conditional("UNITY_EDITOR")]
    public sealed class OnChangedValueForMethodAttribute : PropertyAttribute
    {
        public string[] FieldNames { get; }

        public OnChangedValueForMethodAttribute(params string[] fieldNames)
        {
            FieldNames = fieldNames ?? Array.Empty<string>();
        }
    }
}
