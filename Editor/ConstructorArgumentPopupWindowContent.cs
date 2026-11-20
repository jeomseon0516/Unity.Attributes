#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    /// <summary>
    /// .. 매개변수 생성자 입력 폼을 AdvancedDropdown과 동일하게 버튼에 앵커된 별도의 뜬 창으로
    /// 보여줍니다. 필드 자체를 인라인으로 확장해 그리면 "생성"을 누르기 전인데도 이미 값이 반영된
    /// 것처럼 보일 수 있어, 입력이 끝나지 않은 상태임이 명확하도록 분리했습니다. "생성"을 누르기
    /// 전까지는 SerializedProperty를 전혀 건드리지 않으며, 창 밖을 클릭해 닫아도 아무 것도
    /// 대입되지 않습니다.
    /// </summary>
    internal sealed class ConstructorArgumentPopupWindowContent : PopupWindowContent
    {
        private const float Width = 320f;
        private const float MaxHeight = 240f;
        private const float Padding = 4f;
        private const float TitleHeight = 20f;

        private readonly PendingConstructorSelection _pending;
        private readonly Func<string> _onCreate;
        private Vector2 _scrollPosition;
        private bool _usesUIToolkit;

        public ConstructorArgumentPopupWindowContent(PendingConstructorSelection pending, Func<string> onCreate)
        {
            _pending = pending;
            _onCreate = onCreate;
        }

        public override Vector2 GetWindowSize()
        {
            float errorHeight = string.IsNullOrEmpty(_pending.ErrorMessage) ? 0f : 44f;
            float height = TitleHeight + ConstructorArgumentFormGUI.GetHeight(_pending) + errorHeight + Padding * 2f;
            return new Vector2(Width, Mathf.Min(MaxHeight, height));
        }

        public override VisualElement CreateGUI()
        {
            _usesUIToolkit = true;

            VisualElement root = new();
            root.style.flexGrow = 1f;
            root.style.paddingLeft = Padding;
            root.style.paddingRight = Padding;
            root.style.paddingTop = Padding;
            root.style.paddingBottom = Padding;

            Label title = new($"{_pending.SelectedType.Name} 생성");
            title.style.height = TitleHeight;
            title.style.flexShrink = 0f;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.unityTextAlign = TextAnchor.MiddleLeft;
            root.Add(title);

            ScrollView scrollView = new(ScrollViewMode.Vertical);
            scrollView.style.flexGrow = 1f;
            root.Add(scrollView);

            HelpBox error = new(string.Empty, HelpBoxMessageType.Error);
            error.style.marginTop = 2f;

            scrollView.Add(ConstructorArgumentFormGUI.CreateGUI(_pending, RefreshError, OnAction));
            scrollView.Add(error);
            RefreshError();
            return root;

            void RefreshError()
            {
                error.text = _pending.ErrorMessage ?? string.Empty;
                error.style.display = string.IsNullOrEmpty(_pending.ErrorMessage) ? DisplayStyle.None : DisplayStyle.Flex;
            }

            void OnAction(ConstructorArgumentFormAction action)
            {
                switch (action)
                {
                    case ConstructorArgumentFormAction.Created:
                        string errorMessage = _onCreate();
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            editorWindow.Close();
                        }
                        else
                        {
                            _pending.ErrorMessage = errorMessage;
                            RefreshError();
                        }
                        break;
                    case ConstructorArgumentFormAction.Cancelled:
                        editorWindow.Close();
                        break;
                }
            }
        }

        public override void OnGUI(Rect rect)
        {
            // UI Toolkit 내용이 만들어졌으면 IMGUI 폼을 겹쳐 그리지 않습니다.
            if (_usesUIToolkit) return;

            Rect titleRect = new(rect.x + Padding, rect.y + Padding, rect.width - Padding * 2f, TitleHeight);
            EditorGUI.LabelField(titleRect, $"{_pending.SelectedType.Name} 생성", EditorStyles.boldLabel);

            float contentHeight = ConstructorArgumentFormGUI.GetHeight(_pending);
            if (!string.IsNullOrEmpty(_pending.ErrorMessage)) contentHeight += 44f;

            Rect viewport = new(rect.x + Padding, titleRect.yMax, rect.width - Padding * 2f,
                rect.height - TitleHeight - Padding * 2f);
            Rect content = new(0f, 0f, Mathf.Max(0f, viewport.width - 16f), contentHeight);
            _scrollPosition = GUI.BeginScrollView(viewport, _scrollPosition, content);
            ConstructorArgumentFormAction action = ConstructorArgumentFormGUI.Draw(
                new Rect(0f, 0f, content.width, ConstructorArgumentFormGUI.GetHeight(_pending)),
                _pending,
                editorWindow.Repaint);

            if (!string.IsNullOrEmpty(_pending.ErrorMessage))
            {
                Rect errorRect = new(0f, ConstructorArgumentFormGUI.GetHeight(_pending) + 2f, content.width, 40f);
                EditorGUI.HelpBox(errorRect, _pending.ErrorMessage, MessageType.Error);
            }
            GUI.EndScrollView();

            switch (action)
            {
                case ConstructorArgumentFormAction.Created:
                    string errorMessage = _onCreate();
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        editorWindow.Close();
                    }
                    else
                    {
                        _pending.ErrorMessage = errorMessage;
                        editorWindow.Repaint();
                    }
                    break;
                case ConstructorArgumentFormAction.Cancelled:
                    editorWindow.Close();
                    break;
            }
        }
    }
}
#endif
