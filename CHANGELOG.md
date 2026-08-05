# 변경 기록

## [Unreleased]

- `ComponentDropdown`을 Unity 6 제네릭 `TreeView<int>` API로 전환했습니다.
- `DisplayAsAttribute`를 제거하고 Unity `InspectorNameAttribute`로 대체했습니다.
- `INFO_TYPE`을 `InfoBoxType`으로 변경하고 enum 멤버를 PascalCase로 정리했습니다.
- `Vector2RangeAttribute`를 실제 UI가 드러나는 `Vector2SliderAttribute`로 변경했습니다.
- `InspectorButtonAttribute.ButtonName`을 `Label`로 변경했습니다.
- `InitializeRequireComponentAttribute`를 `GetOrAddComponentAttribute`로 변경하고 타입 검사, Undo 및 Prefab 기록을 수정했습니다.
- `SelectableSerializeFieldAttribute`를 `HierarchyObjectPickerAttribute`로 변경하고 대상별 캐시와 SerializedProperty 수명을 수정했습니다.
- `OnwMinAttribute`를 제거하고 동일한 기능의 Unity `MinAttribute`를 사용하도록 변경했습니다.
- `OnwMaxAttribute`를 의도가 명확한 `MaxValueAttribute`로 변경했습니다.
- `MaxValueAttribute`가 최댓값이 아닌 최솟값을 적용하던 Drawer 결함을 수정하고 float 상한을 지원합니다.
- 계약과 사용처가 불명확한 `OnChangedValueByValueAttribute`와 Drawer를 제거했습니다.
- `OnChangedValueForMethodAttribute`를 Editor 편의 기능임이 드러나는 `InvokeOnInspectorChangeAttribute`로 변경했습니다.
- Editor 메서드 호출 조건을 표현하는 공통 기반 `EditorMethodTriggerAttribute`를 추가하고, `InspectorButtonAttribute`와 `InvokeOnInspectorChangeAttribute`가 이를 상속하도록 변경했습니다.
- 범용 Attribute의 Drawer, Inspector 주입 및 메서드 실행 구현과 관련 샘플을 EditorToolkit에서 이동했습니다.
- 기능 전용 Attribute는 기능 패키지가 선언과 구현을 함께 소유하도록 경계를 확정했습니다.
- Runtime과 Editor asmdef를 분리하고 공통 Editor API만 EditorToolkit에 단방향으로 의존하도록 구성했습니다.
- `StringBuilderPool` 이동에 맞춰 `Jeomseon.Text`를 사용하고 GameObject Pooling 패키지 의존성을 제거했습니다.
- `InvokeOnInspectorChangeAttribute` 메서드 탐색을 Unity `TypeCache` 기반 전용 캐시로 변경하고, 상속된 private 메서드와 override 중복을 일관되게 처리합니다.
- `InspectorButtonAttribute`와 `InvokeOnInspectorChangeAttribute`가 TypeCache 탐색, 상속 처리, 매개변수 메타데이터 및 안전한 메서드 호출 경로를 공유하도록 공통화했습니다.
- 공통 메서드 메타데이터와 캐시를 `EditorMethodTriggerAttribute` 기반으로 제한하고, 기능별 Attribute를 Trigger 메타데이터로 처리하도록 변경했습니다.
- Trigger 메서드 명칭을 `EditorTriggeredMethod`로 통일하고, 공통 매개변수 메타데이터·검증기·호출 요청 객체 및 Trigger Handler Registry를 추가했습니다.
- 공개 Attribute 계약, `MaxValueAttribute` 상한 처리 및 값 변경 호출 중복 방지 EditMode 테스트를 추가했습니다.
- `GetOrAddComponent`와 `HierarchyObjectPicker`를 직접 검증하는 Object Assignment Sample Scene을 추가했습니다.

## [0.2.2] - 2026-07-29

- Runtime·Samples 어셈블리의 `rootNamespace`와 Attribute 파일 위치를 namespace에 맞게 정리했습니다.

## [0.2.1] - 2026-07-29

- 범용 Attribute 선언 사용법을 확인하는 `Basic Usage` 샘플을 추가했습니다.

## [0.2.0] - 2026-07-29

### Changed

- `OnChangedValueByMethodAttribute`를 의도가 더 명확한 `OnChangedValueForMethodAttribute`로 변경했습니다.
- 초기 배포 단계이므로 구 이름의 호환 타입은 남기지 않았습니다.

## [0.1.0] - 2026-07-29

### Added

- JeomseonScriptPack의 범용 Inspector Attribute 선언을 독립 패키지로 분리했습니다.
- Localization 전용 Attribute는 Localization 패키지가 소유하도록 제외했습니다.
