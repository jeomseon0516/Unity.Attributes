#if UNITY_EDITOR
namespace Jeomseon.Attribute.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// Inspector Injection 영역에서 InspectorButton GUI를 호출합니다.
    /// </summary>
    internal sealed class InspectorButtonInjectedDrawer
        : IInspectorInjectedDrawer, IInspectorButtonPresenter
    {
        public void OnEnable(UnityEditorObjectEditor editor)
        {
        }

        public void OnInspectorGUI(UnityEditorObjectEditor editor)
        {
            Draw(editor);
        }

        public void Draw(UnityEditorObjectEditor editor) => InspectorButtonGUI.Draw(editor);

        public void Dispose()
        {
        }
    }
}
#endif
