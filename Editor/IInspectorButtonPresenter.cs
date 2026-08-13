#if UNITY_EDITOR
namespace Jeomseon.Unity.Attributes.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// InspectorButton을 표시하는 호스트 표면의 공통 계약입니다.
    /// </summary>
    internal interface IInspectorButtonPresenter
    {
        void Draw(UnityEditorObjectEditor editor);
    }
}
#endif
