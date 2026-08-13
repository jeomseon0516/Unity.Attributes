#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    /// <summary>
    /// .. SerializeReferenceSelector가 매개변수 생성자를 가진 타입을 생성할 때, 생성자 매개변수
    /// 하나의 Editor 입력 필드를 그리고 값을 제공합니다. 지원할 매개변수 타입마다 구현체를
    /// 추가하며, TypeCache로 자동 검색되므로 등록 절차는 따로 없습니다.
    /// </summary>
    public interface ISerializeReferenceSelectorConstructorParameterPipeline
    {
        bool CanHandle(Type parameterType);
        object GetDefaultValue(Type parameterType);
        object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue);
    }
}
#endif
