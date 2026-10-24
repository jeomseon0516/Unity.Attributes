# Jeomseon Unity Attributes

[English](Overview.en.md) · [설치](../README.md)

## 목적

Inspector 표시·값 제한·참조 연결·메서드 실행·SerializeReference 생성을 Attribute로 구성합니다. 선언과 Editor 구현을 어셈블리로 분리합니다.

## 사용 흐름

필드/메서드에 Attribute 선언 → Inspector에서 편집 또는 실행 → SerializedProperty/Undo 경로로 반영. SerializeReference 생성자 인자는 별도 팝업에서 확인 후 적용합니다.

## 계약과 제한

ReadOnly는 런타임 접근 제어가 아닙니다. 변경 콜백은 Undo 편집 스트림을 관찰합니다. 생성자의 모든 인자 타입에 입력 파이프라인이 있어야 합니다.

## 패키지 요구 사항

- Package ID: `com.jeomseon.unity.attributes`
- Version: `0.4.2`
- Unity: `6000.6` / `0f1`
- `com.jeomseon.unity.editor-toolkit`: `0.7.2`
- `com.jeomseon.unity.core`: `0.3.2`

## 샘플과 검증

Package Manager > Samples에서 다음 예제를 Import하세요. 포함된 Scene 또는 Editor 도구를 열고 샘플 README의 Setup 절차를 먼저 수행합니다.

- [Basic Usage](../Samples~/BasicUsage/README.md)
  - [InspectorButtonSample](../Samples~/BasicUsage/InspectorButtonSample.unity)
  - [ObjectAssignmentSample](../Samples~/BasicUsage/ObjectAssignmentSample.unity)
  - [SerializeReferenceSelectorSample](../Samples~/BasicUsage/SerializeReferenceSelectorSample.unity)

[테스트 코드](../Tests)

소비 프로젝트 manifest의 `testables`에 패키지 ID를 추가하고 Unity Test Runner의 관련 어셈블리를 실행합니다. 위 링크는 테스트 위치이며 이번 문서 수정으로 새 테스트 실행을 주장하지 않습니다.

## 코드 지도

- [EditorMethodTriggerAttribute](../Runtime/EditorMethodTriggerAttribute.cs)
- [GetOrAddComponentAttribute](../Runtime/GetOrAddComponentAttribute.cs)
- [HierarchyObjectPickerAttribute](../Runtime/HierarchyObjectPickerAttribute.cs)
- [InfoBoxAttribute, InfoBoxType](../Runtime/InfoBoxAttribute.cs)
- [InspectorButtonAttribute](../Runtime/InspectorButtonAttribute.cs)
- [InvokeOnInspectorChangeAttribute](../Runtime/InvokeOnInspectorChangeAttribute.cs)
- [MaxValueAttribute](../Runtime/MaxValueAttribute.cs)
- [ReadOnlyAttribute](../Runtime/ReadOnlyAttribute.cs)
- [SerializeReferenceSelectorAttribute](../Runtime/SerializeReferenceSelectorAttribute.cs)
- [SpritePreviewAttribute](../Runtime/SpritePreviewAttribute.cs)
- [Vector2SliderAttribute](../Runtime/Vector2SliderAttribute.cs)
- [ISerializeReferenceSelectorConstructorParameterPipeline](../Editor/ConstructorPipelines/ISerializeReferenceSelectorConstructorParameterPipeline.cs)
- [SerializeReferenceSelectorConstructorPipelineNameAttribute](../Editor/ConstructorPipelines/SerializeReferenceSelectorConstructorPipelineNameAttribute.cs)
- [InspectorButtonGUI](../Editor/InspectorButtonGUI.cs)

[사용 가이드](Guide.md)
