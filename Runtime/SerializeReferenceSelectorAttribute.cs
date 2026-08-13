using System;
using System.Diagnostics;
using UnityEngine;

namespace Jeomseon.Unity.Attributes
{
    /// <summary>
    /// .. [SerializeReference] 필드/리스트에 붙이면 Inspector에 구체 타입을 선택할 수 있는
    /// 검색 가능한 드롭다운(AdvancedDropdown)을 그려줍니다. [SerializeReference]만으로는
    /// Unity 기본 Inspector가 타입을 고를 UI를 전혀 제공하지 않습니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true), Conditional("UNITY_EDITOR")]
    public sealed class SerializeReferenceSelectorAttribute : PropertyAttribute { }
}
