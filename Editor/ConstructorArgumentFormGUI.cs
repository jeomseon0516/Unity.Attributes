#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Jeomseon.Unity.Attributes.Editor.ConstructorPipelines;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor
{
    /// <summary>
    /// .. SerializeReferenceSelectorDrawer가 매개변수 생성자를 가진 타입을 선택했을 때 보여주는
    /// 임시 입력 폼을 그립니다. "생성" 버튼을 누르기 전까지는 SerializedProperty를 전혀 건드리지
    /// 않습니다 — 생성자 인자 편집은 PendingConstructorSelection의 로컬 버퍼에서만 이뤄집니다.
    /// </summary>
    internal static class ConstructorArgumentFormGUI
    {
        private const float PipelinePickerWidth = 18f;
        private const float ButtonSpacing = 4f;

        public static float GetHeight(PendingConstructorSelection pending)
        {
            bool hasOverloadChoice = pending.ConstructibleConstructors.Length > 1;
            float height = 0f;

            if (hasOverloadChoice)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!pending.ChosenConstructorIndex.HasValue)
            {
                // 아직 오버로드를 고르지 않은 상태 — 취소 버튼 한 줄만 더 그립니다.
                return height + EditorGUIUtility.singleLineHeight;
            }

            int parameterCount = pending.ChosenConstructor.GetParameters().Length;
            height += parameterCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            height += EditorGUIUtility.singleLineHeight; // 생성 / 취소 버튼 행

            return height;
        }

        // onStateChanged: GenericMenu 항목 선택은 다음 이벤트 루프에서 콜백으로 실행되므로, 팝업
        // 창처럼 이 프레임 이후에도 계속 열려 있는 호스트는 상태가 바뀐 시점에 명시적으로 다시
        // 그리도록 알려줘야 합니다(예: PopupWindowContent.editorWindow.Repaint).
        public static ConstructorArgumentFormAction Draw(Rect position, PendingConstructorSelection pending, Action onStateChanged)
        {
            float y = position.y;
            bool hasOverloadChoice = pending.ConstructibleConstructors.Length > 1;

            if (hasOverloadChoice)
            {
                Rect overloadRect = new(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                DrawOverloadPicker(overloadRect, pending, onStateChanged);
                y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                Rect signatureRect = new(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(signatureRect, DescribeConstructor(pending.ConstructibleConstructors[0]));
                y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!pending.ChosenConstructorIndex.HasValue)
            {
                Rect cancelOnlyRect = new(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                return GUI.Button(cancelOnlyRect, "취소")
                    ? ConstructorArgumentFormAction.Cancelled
                    : ConstructorArgumentFormAction.None;
            }

            ParameterInfo[] parameters = pending.ChosenConstructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                Rect paramRect = new(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                DrawParameterField(paramRect, pending, parameters[i], i, onStateChanged);
                y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            Rect buttonRowRect = new(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            float createWidth = buttonRowRect.width * 0.5f - ButtonSpacing * 0.5f;
            Rect createRect = new(buttonRowRect.x, buttonRowRect.y, createWidth, buttonRowRect.height);
            Rect cancelRect = new(createRect.xMax + ButtonSpacing, buttonRowRect.y,
                buttonRowRect.width - createWidth - ButtonSpacing, buttonRowRect.height);

            if (GUI.Button(createRect, "생성")) return ConstructorArgumentFormAction.Created;
            if (GUI.Button(cancelRect, "취소")) return ConstructorArgumentFormAction.Cancelled;

            return ConstructorArgumentFormAction.None;
        }

        private static void DrawOverloadPicker(Rect position, PendingConstructorSelection pending, Action onStateChanged)
        {
            string buttonLabel = pending.ChosenConstructorIndex.HasValue
                ? DescribeConstructor(pending.ConstructibleConstructors[pending.ChosenConstructorIndex.Value])
                : "생성자 선택";

            if (!EditorGUI.DropdownButton(position, new GUIContent(buttonLabel), FocusType.Keyboard)) return;

            GenericMenu menu = new();
            for (int i = 0; i < pending.ConstructibleConstructors.Length; i++)
            {
                int capturedIndex = i;
                string label = DescribeConstructor(pending.ConstructibleConstructors[i]);
                menu.AddItem(new GUIContent(label), pending.ChosenConstructorIndex == capturedIndex, () =>
                {
                    ConstructorSelectionService.ChooseConstructor(pending, capturedIndex);
                    onStateChanged?.Invoke();
                });
            }
            menu.ShowAsContext();
        }

        private static void DrawParameterField(Rect position, PendingConstructorSelection pending, ParameterInfo parameter, int parameterIndex, Action onStateChanged)
        {
            IReadOnlyList<ISerializeReferenceSelectorConstructorParameterPipeline> candidates =
                ConstructorParameterPipelineRegistry.GetCandidates(parameter.ParameterType);

            if (!pending.ParameterPipelineChoice.TryGetValue(parameterIndex, out int chosenCandidateIndex) ||
                chosenCandidateIndex >= candidates.Count)
            {
                chosenCandidateIndex = 0;
            }

            Rect fieldRect = position;
            if (candidates.Count > 1)
            {
                Rect pickerRect = new(position.xMax - PipelinePickerWidth, position.y, PipelinePickerWidth, position.height);
                fieldRect = new Rect(position.x, position.y, position.width - PipelinePickerWidth - ButtonSpacing, position.height);
                DrawPipelinePicker(pickerRect, pending, parameterIndex, candidates, chosenCandidateIndex, onStateChanged);
            }

            GUIContent label = new(parameter.Name);
            pending.ParameterValues[parameterIndex] = candidates[chosenCandidateIndex]
                .DrawField(fieldRect, label, parameter.ParameterType, pending.ParameterValues[parameterIndex]);
        }

        private static void DrawPipelinePicker(
            Rect position,
            PendingConstructorSelection pending,
            int parameterIndex,
            IReadOnlyList<ISerializeReferenceSelectorConstructorParameterPipeline> candidates,
            int chosenCandidateIndex,
            Action onStateChanged)
        {
            if (!EditorGUI.DropdownButton(position, GUIContent.none, FocusType.Passive)) return;

            GenericMenu menu = new();
            for (int i = 0; i < candidates.Count; i++)
            {
                int capturedIndex = i;
                string name = ConstructorParameterPipelineRegistry.GetDisplayName(candidates[i]);
                menu.AddItem(new GUIContent(name), i == chosenCandidateIndex, () =>
                {
                    pending.ParameterPipelineChoice[parameterIndex] = capturedIndex;
                    onStateChanged?.Invoke();
                });
            }
            menu.ShowAsContext();
        }

        private static string DescribeConstructor(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            string[] parameterDescriptions = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterDescriptions[i] = $"{parameters[i].ParameterType.Name} {parameters[i].Name}";
            }

            return $"{constructor.DeclaringType?.Name}({string.Join(", ", parameterDescriptions)})";
        }
    }
}
#endif
