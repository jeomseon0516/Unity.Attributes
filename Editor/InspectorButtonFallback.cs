#if UNITY_EDITOR
using UnityEditor;
using UnityEditorObjectEditor = global::UnityEditor.Editor;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Inspector Injection을 사용할 수 없을 때 Inspector 헤더에 버튼을 표시합니다.
    /// </summary>
    [InitializeOnLoad]
    internal static class InspectorButtonFallback
    {
        private static readonly IInspectorButtonPresenter Presenter =
            new InspectorButtonHeaderPresenter();

        static InspectorButtonFallback()
        {
            Refresh();
            AssemblyReloadEvents.beforeAssemblyReload += Dispose;
            EditorApplication.quitting += Dispose;
        }

        internal static void Refresh()
        {
            global::UnityEditor.Editor.finishedDefaultHeaderGUI -= OnFinishedDefaultHeaderGUI;
            global::UnityEditor.Editor.finishedDefaultHeaderGUI += OnFinishedDefaultHeaderGUI;
        }

        private static void OnFinishedDefaultHeaderGUI(UnityEditorObjectEditor editor)
        {
            if (InspectorInjectionService.IsRunning)
                return;

            Presenter.Draw(editor);
        }

        private static void Dispose()
        {
            global::UnityEditor.Editor.finishedDefaultHeaderGUI -= OnFinishedDefaultHeaderGUI;
        }

        private sealed class InspectorButtonHeaderPresenter : IInspectorButtonPresenter
        {
            public void Draw(UnityEditorObjectEditor editor) => InspectorButtonGUI.DrawFallback(editor);
        }
    }
}
#endif
