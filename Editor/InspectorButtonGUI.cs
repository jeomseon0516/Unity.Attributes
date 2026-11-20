#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// InspectorButtonAttribute가 지정된 메서드의 버튼을 인스펙터 본문 아래에 그립니다.
    /// Inspector Injection 백엔드와 자체 CustomEditor의 명시적 호출이 이 진입점을 함께 사용합니다.
    /// IMGUI 호스트(OnInspectorGUI)는 <see cref="Draw"/>, UI Toolkit 호스트(CreateInspectorGUI)는
    /// <see cref="CreateGUI"/>를 사용합니다.
    /// </summary>
    public static class InspectorButtonGUI
    {
        // EditorGUILayout.Space()의 기본 간격과 같습니다.
        private const float SpaceHeight = 6f;

        /// <summary>
        /// <see cref="Draw"/>와 같은 버튼 목록을 UI Toolkit 요소로 만듭니다.
        /// 표시할 버튼이 없으면 자식이 없는 빈 요소를 반환합니다.
        /// </summary>
        public static VisualElement CreateGUI(UnityEditorObjectEditor editor)
        {
            VisualElement root = new();
            if (editor == null || editor.target == null)
                return root;

            InspectorButtonMethod[] methods =
                InspectorButtonMethodCache.Get(editor.target.GetType());
            if (methods.Length == 0)
                return root;

            root.style.marginTop = SpaceHeight;

            foreach (InspectorButtonMethod item in methods)
            {
                MethodInfo method = item.Method;
                root.Add(new Button(() => InspectorButtonInvoker.InvokeForAllTargets(editor, method))
                {
                    text = item.Label
                });
            }

            return root;
        }

        public static void Draw(UnityEditorObjectEditor editor)
        {
            if (editor == null || editor.target == null)
                return;

            InspectorButtonMethod[] methods =
                InspectorButtonMethodCache.Get(editor.target.GetType());
            if (methods.Length == 0)
                return;

            EditorGUILayout.Space();

            foreach (InspectorButtonMethod item in methods)
            {
                if (GUILayout.Button(item.Label))
                    InspectorButtonInvoker.InvokeForAllTargets(editor, item.Method);
            }
        }

        internal static void DrawFallback(UnityEditorObjectEditor editor)
        {
            if (editor == null || editor.target == null)
                return;

            if (editor.target is GameObject gameObject)
            {
                DrawGameObjectFallback(editor, gameObject);
                return;
            }

            InspectorButtonMethod[] methods =
                InspectorButtonMethodCache.Get(editor.target.GetType());
            if (methods.Length == 0)
                return;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Jeomseon Inspector Buttons", EditorStyles.boldLabel);
            foreach (InspectorButtonMethod item in methods)
            {
                if (GUILayout.Button(item.Label))
                    InspectorButtonInvoker.InvokeForAllTargets(editor, item.Method);
            }

            EditorGUILayout.EndVertical();
        }

        private static void DrawGameObjectFallback(
            UnityEditorObjectEditor editor,
            GameObject gameObject)
        {
            foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
            {
                if (component == null)
                    continue;

                InspectorButtonMethod[] methods = InspectorButtonMethodCache.Get(component.GetType());
                if (methods.Length == 0)
                    continue;

                foreach (InspectorButtonMethod item in methods)
                {
                    if (!GUILayout.Button($"{component.GetType().Name}: {item.Label}"))
                        continue;

                    List<UnityEngine.Object> targets = new();
                    foreach (UnityEngine.Object target in editor.targets)
                    {
                        if (target is GameObject selected)
                            targets.AddRange(selected.GetComponents(component.GetType()));
                    }

                    InspectorButtonInvoker.InvokeForTargets(targets, item.Method);
                }
            }
        }
    }
}
#endif
