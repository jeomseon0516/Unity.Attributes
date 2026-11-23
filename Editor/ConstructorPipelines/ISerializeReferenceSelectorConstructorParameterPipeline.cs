#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    /// <summary>
    /// .. SerializeReferenceSelector가 매개변수 생성자를 가진 타입을 생성할 때, 생성자 매개변수
    /// 하나의 Editor 입력 필드를 그리고 값을 제공합니다. 지원할 매개변수 타입마다 구현체를
    /// 추가하며, TypeCache로 자동 검색되므로 등록 절차는 따로 없습니다.
    /// IMGUI 입력 폼은 <see cref="DrawField"/>, UI Toolkit 입력 폼은 <see cref="CreateField"/>를 사용합니다.
    /// </summary>
    public interface ISerializeReferenceSelectorConstructorParameterPipeline
    {
        bool CanHandle(Type parameterType);
        object GetDefaultValue(Type parameterType);
        object DrawField(Rect position, GUIContent label, Type parameterType, object currentValue);

        /// <summary>
        /// UI Toolkit 입력 폼에 표시할 필드를 만듭니다. 값이 바뀔 때마다 <paramref name="onValueChanged"/>로
        /// 새 값을 전달합니다. 재정의하지 않은 구현체는 <see cref="DrawField"/>를 IMGUIContainer로 감싸 표시합니다.
        /// </summary>
        VisualElement CreateField(string label, Type parameterType, object currentValue, Action<object> onValueChanged)
        {
            object value = currentValue;
            GUIContent content = new(label);
            return new IMGUIContainer(() =>
            {
                Rect position = EditorGUILayout.GetControlRect();
                object newValue = DrawField(position, content, parameterType, value);
                if (Equals(newValue, value))
                    return;

                value = newValue;
                onValueChanged?.Invoke(newValue);
            });
        }
    }
}
#endif
