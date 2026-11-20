#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    /// <summary>
    /// EditorGUI.DropdownButton과 같은 팝업 버튼 모양의 UI Toolkit 요소입니다.
    /// 클릭하면 메뉴·드롭다운을 띄울 기준 영역(worldBound)을 전달합니다.
    /// </summary>
    internal sealed class DropdownButtonElement : VisualElement
    {
        private readonly TextElement _text;

        public event Action<Rect> Clicked;

        public DropdownButtonElement(string text)
        {
            AddToClassList(BasePopupField<string, string>.inputUssClassName);
            focusable = true;

            _text = new TextElement();
            _text.AddToClassList(BasePopupField<string, string>.textUssClassName);
            Add(_text);

            VisualElement arrow = new();
            arrow.AddToClassList(BasePopupField<string, string>.arrowUssClassName);
            Add(arrow);

            Text = text;
            this.AddManipulator(new Clickable(() => Clicked?.Invoke(worldBound)));
        }

        public string Text
        {
            get => _text.text;
            set => _text.text = value ?? string.Empty;
        }
    }
}
#endif
