#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// Inspector Injection 영역(UI Toolkit)에 InspectorButton 요소를 만듭니다.
    /// </summary>
    internal sealed class InspectorButtonInjectedDrawer
        : IInspectorInjectedDrawer, IInspectorButtonPresenter
    {
        public void OnEnable(UnityEditorObjectEditor editor)
        {
        }

        public VisualElement CreateInspectorGUI(UnityEditorObjectEditor editor)
            => InspectorButtonGUI.CreateGUI(editor);

        public void Draw(UnityEditorObjectEditor editor) => InspectorButtonGUI.Draw(editor);

        public void Dispose()
        {
        }
    }
}
#endif
