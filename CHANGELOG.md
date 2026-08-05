# 변경 기록

모든 주요 변경 사항은 이 파일에 기록됩니다.

## [0.3.0] - 2026-08-05

### Breaking Changes

- `DisplayAsAttribute`를 제거하고 Unity `InspectorNameAttribute`를 사용하도록 변경했습니다.
- `INFO_TYPE`을 `InfoBoxType`으로 변경하고 enum 멤버를 PascalCase로 정리했습니다.
- `Vector2RangeAttribute`를 `Vector2SliderAttribute`로 변경했습니다.
- `InspectorButtonAttribute.ButtonName`을 `Label`로 변경했습니다.
- `InitializeRequireComponentAttribute`를 `GetOrAddComponentAttribute`로 변경했습니다.
- `SelectableSerializeFieldAttribute`를 `HierarchyObjectPickerAttribute`로 변경했습니다.
- `OnwMinAttribute`를 제거하고 Unity `MinAttribute`를 사용하도록 변경했습니다.
- `OnwMaxAttribute`를 `MaxValueAttribute`로 변경했습니다.
- `OnChangedValueByValueAttribute` 및 관련 Drawer를 제거했습니다.
- `OnChangedValueForMethodAttribute`를 `InvokeOnInspectorChangeAttribute`로 변경했습니다.

### Added

- `EditorMethodTriggerAttribute` 공통 기반 Attribute를 추가했습니다.
- Trigger Handler Registry 및 공통 Trigger 메타데이터를 추가했습니다.
- 공개 Attribute 계약 및 EditMode 테스트를 추가했습니다.
- `GetOrAddComponent` 및 `HierarchyObjectPicker` 검증용 Sample Scene을 추가했습니다.

### Changed

- `ComponentDropdown`을 Unity 6 제네릭 `TreeView<int>` API로 전환했습니다.
- Runtime과 Editor asmdef를 분리하고 의존성을 재구성했습니다.
- 범용 Attribute의 Drawer 및 Inspector 구현을 EditorToolkit으로 이동했습니다.
- 기능별 Attribute는 해당 기능 패키지가 직접 소유하도록 패키지 경계를 정리했습니다.
- `StringBuilderPool` 이동에 맞춰 `Jeomseon.Text`를 사용하도록 변경했습니다.
- `InspectorButtonAttribute`와 `InvokeOnInspectorChangeAttribute`가 공통 Trigger 기반 구현을 공유하도록 리팩터링했습니다.
- `InvokeOnInspectorChangeAttribute`의 메서드 탐색을 Unity `TypeCache` 기반 캐시로 개선했습니다.
- Trigger 메서드 명칭을 `EditorTriggeredMethod`로 통일했습니다.

### Fixed

- `GetOrAddComponentAttribute`의 타입 검사, Undo 및 Prefab 기록을 수정했습니다.
- `HierarchyObjectPickerAttribute`의 대상별 캐시와 `SerializedProperty` 수명 문제를 수정했습니다.
- `MaxValueAttribute`가 최솟값을 적용하던 문제를 수정하고 float 상한을 지원하도록 개선했습니다.
- 값 변경 시 Trigger 메서드가 중복 호출될 수 있는 문제를 수정했습니다.

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


## [0.3.1] - 2026-08-05

- Unity 6000.5.7f1을 최소 지원 버전으로 상향했습니다.
