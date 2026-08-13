# 변경 기록

모든 주요 변경 사항은 이 파일에 기록됩니다.

## [0.4.0] - 2026-08-13

- **사용자 Unity 실행 검증 통과**: `SerializeReferenceSelectorSample` Scene에서
  `StunAttributeEffect`/`ElementalAttributeEffect`로 오버로드 선택, 매개변수 파이프라인
  전 종류(int/float/double/bool/string/enum/`UnityEngine.Object`/Vector2/Vector3/Vector4/Color),
  선택적 기본값, 취소 시 불변, 생성자 예외 표시, 다중 선택 독립 인스턴스와 Undo를 확인했습니다
  (ROADMAP P3-04 완료).
- `SerializeReferenceSelector` 생성자 입력 팝업에 시그니처, 긴 폼 스크롤과 생성 실패 메시지를
  추가했습니다. 다중 오브젝트 편집은 대상별 독립 인스턴스를 생성하며, 어느 생성자라도 실패하면
  기존 값을 모두 유지합니다.
- 생성자 오버로드·기본값·파이프라인·취소 전 불변·Undo·다중 선택·예외 계약의 EditMode 테스트와
  Object/Enum/Vector/선택적 기본값 Sample 검증 절차를 추가했습니다. `ElementalAttributeEffect`
  Sample 타입을 추가해 `StunAttributeEffect`가 다루지 않던 나머지 기본 제공 파이프라인(int/bool/
  double/Color/Vector2/Vector4)과 생성자 예외 표시를 Sample에서도 확인할 수 있게 했습니다.

- **(Breaking)** 네임스페이스를 `Jeomseon.Attribute`(Runtime)/`Jeomseon.Attribute.Editor`(Editor) →
  `Jeomseon.Unity.Attributes`/`Jeomseon.Unity.Attributes.Editor`로 변경했습니다(하위
  `.ConstructorPipelines`도 동일). 워크스페이스 전체 네임스페이스 규칙(`AGENTS.md` 참고)을 적용한
  것으로, 폴더 구조 변경은 없습니다. `Tests`의 `Jeomseon.Attribute.Tests`는 이번 규칙 적용 범위 밖이라
  그대로 두었지만, 기존에 `Jeomseon.Attribute`의 하위 네임스페이스라는 점을 이용해 `using` 없이
  Runtime/Editor 타입을 암시적으로 찾던 테스트 파일 다수가 이번 변경으로 더 이상 부모-자식 관계가
  아니게 되어 컴파일이 깨졌습니다(`AttributeContractTests`, `EditorImplementationTests`,
  `SelectorManagedReferenceTestComponent`) — 명시적 `using Jeomseon.Unity.Attributes;`를 추가해
  수정했습니다.

- `[SerializeReferenceSelector]`를 추가했습니다. `[SerializeReference]` 단일 필드와 리스트에서
  검색 가능한 구체 타입 드롭다운, `(None)`, Undo를 지원합니다.
- Play Mode 종료 후 managed-reference 복원을 비교하는 Plain/Selector 회귀 테스트와 실제 타입·값의
  Scene 저장·재오픈 및 Play Mode 복원을 확인하는 Sample Scene을 추가했습니다.
- `[SerializeReferenceSelector]`가 매개변수 있는 생성자를 지원합니다(ROADMAP P3-04). 타입 선택 시
  생성 가능한 생성자가 매개변수 없는 것 하나뿐이면 기존과 동일하게 즉시 생성하고, 그 외(오버로드가
  여럿이거나 유일한 생성자가 매개변수를 요구하는 경우)에는 타입 버튼에 앵커된 AdvancedDropdown과
  동일한 방식의 뜬 팝업 창(`PopupWindowContent`)에서 값을 다 채우고 "생성"을 눌러야 실제로
  대입·Undo 기록됩니다. 필드를 인라인으로 확장해 입력 폼을 그리면 아직 입력 중인데도 이미 반영된
  것처럼 보일 수 있어, SerializedProperty를 전혀 건드리지 않는 별도 창으로 분리했습니다(창 밖을
  클릭해 닫아도 아무 것도 대입되지 않음). 오버로드가 여럿이면 드롭다운으로 직접 고릅니다. 매개변수
  타입별 Editor 필드는
  `ISerializeReferenceSelectorConstructorParameterPipeline` 구현체가 담당하며(int/float/double/
  bool/string/enum/`UnityEngine.Object` 참조/`Vector2`/`Vector3`/`Vector4`/`Color` 기본 제공),
  TypeCache로 자동 검색되어 소비 패키지가 자체 파이프라인을 추가할 수 있습니다. 같은 매개변수
  타입을 처리하는 파이프라인이 둘 이상이면 필드 옆에 선택 버튼이 추가로 뜨며,
  `[SerializeReferenceSelectorConstructorPipelineName("...")]`로 표시 이름을 지정할 수 있습니다
  (없으면 구현체 클래스 이름을 그대로 사용).

## [0.3.2] - 2026-08-11

- 워크스페이스 명명 규칙에 맞춰 `[SerializeField] private` 필드를 `_camelCase`에서 `camelCase`로
  정리하고 기존 이름을 `[FormerlySerializedAs]`로 보존했습니다. Editor 내부 `private static
  readonly` 캐시 필드(`InspectorButtonMethodCache`, `InvokeOnInspectorChangeProcessor`)도
  `_camelCase`로 정리했습니다. 공개 C# API 변경은 없으며 기존 Scene·Prefab의 직렬화된 값은
  그대로 유지됩니다.

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
