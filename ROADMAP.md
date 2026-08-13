# Attributes 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 작업 순서

1. **P0-03 — Unity 6000.5 Inspector Injection 안정화 (완료)**
   - Package Manager Sample Reimport 후 Attributes Sample GUID와 Scene 참조를 검증했습니다.
   - Injection 비활성화·Assembly Reload 시 기존 컨테이너를 즉시 제거하고 Header fallback으로 전환합니다.
   - Unity 6000.5의 `Object.GetEntityId()` API에 맞춰 Editor 식별자 처리를 갱신했습니다.
   - Injection 활성 버튼과 비활성 Header fallback 버튼의 표시·호출을 수동 검증했습니다.
   - Attributes EditMode 전체 테스트를 GUI Test Runner에서 28/28 통과했습니다.
   - 사용자 수동 검증으로 UI Toolkit `InfoBox`·`MaxValue`·`ReadOnly`, 좁은 Inspector 폭의
     InfoBox 줄바꿈, Domain Reload/Assembly Reload 조합의 Injection 중복·누수·재활성화를 확인했습니다.
2. **P1-01 — Attribute 소유권과 의존성 경계 확정 (완료)**
   - 범용 Attribute 선언과 Editor 구현을 이 패키지에 함께 유지합니다.
   - 기능 전용 Attribute 선언과 Editor 구현은 해당 기능 패키지가 소유합니다.
   - Runtime 어셈블리는 Editor 의존성을 갖지 않고, Editor asmdef만 EditorToolkit 공통 API를 단방향으로 참조합니다.
   - `LocalizedStringAttribute`와 `ResetOnPoolReleaseAttribute`가 각 기능 패키지에 있음을 확인했습니다.
   - EditorToolkit의 Attribute 구현과 샘플을 이 패키지로 이동하고 역방향 의존성을 제거했습니다.
3. **P1-02 — 공개 API 이름 정리 (완료)**
   - `OnwMin`은 Unity `MinAttribute`로 대체하고 `OnwMax`는 `MaxValueAttribute`로 변경했습니다.
   - 사용 중단된 비제네릭 TreeView API를 Unity 6 `TreeView<int>` API로 교체했습니다.
   - `OnChangedValueByValue`를 제거하고 `OnChangedValueForMethod`를 `InvokeOnInspectorChange`로 변경했습니다.
   - `InfoBoxType`, `Vector2Slider`, `GetOrAddComponent`, `HierarchyObjectPicker`로 공개 API 이름을 정리했습니다.
   - Unity `InspectorNameAttribute`와 중복되는 `DisplayAsAttribute`를 제거했습니다.
4. **P2-01 — Attribute 계약 테스트 (완료)**
   - `AttributeUsage`, 상속, 다중 적용 및 Conditional 동작을 테스트합니다.
   - `MaxValueAttribute` 상한 처리와 `InvokeOnInspectorChange` 호출 중복 방지를 테스트합니다.
5. **P3-01 — Source Generator 가능성 검토**
   - Reflection 기반 처리 비용이 큰 기능만 생성 코드로 대체할 가치가 있는지 평가합니다.

## 추가 후속 작업

### P2-03 — 공개 Sample 보강 (완료)

- UI Toolkit Inspector를 명시적으로 사용하는 `AttributesSample` Scene을 추가합니다.
- InfoBox·MaxValue·ReadOnly를 한 화면에서 확인할 수 있는 재현 가능한 수동 검증 절차를 README에 기록합니다.
- Injection 활성/비활성, Assembly Reload, Domain Reload 조합을 반복 실행하는 검증 체크리스트를 Sample 문서에 추가합니다.
- `SerializeReferenceSelectorSample` Scene에서 단일·리스트 구체 타입 선택, Scene 재오픈 후 직렬화 유지,
  Play Mode 종료 후 Edit Mode 값 복원을 검증합니다.

### P3-02 — API·문서 정리 (완료)

- 공개 Attribute별 지원 필드 타입, 호출 시점, Undo/Prefab 동작을 한·영 README 표로 정리합니다.
- 기존 0.x API 제거 목록과 마이그레이션 예시를 CHANGELOG에 보강합니다.
- `InspectorButton` 메서드 인자 지원 범위와 향후 입력 UI 계획을 명시합니다.

### P3-03 — Source Generator 타당성 재평가 (추후 작업)

- 실제 프로젝트에서 Reflection 비용과 Inspector 갱신 빈도를 측정합니다.
- 측정 결과가 유의미할 때만 생성 코드 도입 범위와 Unity Editor 호환 전략을 설계합니다.

### P3-04 — SerializeReferenceSelector 매개변수 생성자 지원 (완료, Unity 실행 검증 통과)

- 타입 선택 시 생성 가능한(모든 매개변수를 파이프라인이 지원하는) 생성자가 매개변수 없는 것
  하나뿐이면 기존과 동일하게 `ConstructorInfo.Invoke`로 즉시 생성합니다. 그 외에는 타입 버튼에
  앵커된 AdvancedDropdown과 동일한 방식의 `ConstructorArgumentPopupWindowContent` 팝업 창이 뜨고,
  값을 다 채운 뒤 "생성" 버튼을 눌러야 `managedReferenceValue`에 1회만 대입·Undo 기록됩니다(입력
  도중에는 SerializedProperty를 전혀 건드리지 않으며, 창 밖을 클릭해 닫아도 아무 것도 대입되지
  않음). 필드를 인라인으로 확장해 그리는 대신 별도 창으로 분리한 이유는, 인라인 폼이 아직 입력이
  끝나지 않았는데도 이미 값이 반영된 것처럼 보이는 UX 모호함이 있었기 때문입니다.
- 생성자 오버로드가 여럿이면 `ConstructorArgumentFormGUI`가 드롭다운으로 직접 고르게 합니다.
- 매개변수별 Inspector 입력 UI는 `ISerializeReferenceSelectorConstructorParameterPipeline`
  구현체가 담당하며 TypeCache로 자동 검색됩니다. int/float/double/bool/string/enum/
  `UnityEngine.Object` 참조/`Vector2`/`Vector3`/`Vector4`/`Color`를 기본 제공하고, 소비 패키지가
  자체 타입을 위한 파이프라인을 추가할 수 있습니다. 같은 매개변수 타입을 처리하는 파이프라인이
  둘 이상이면 필드 옆 선택 버튼으로 고르며,
  `[SerializeReferenceSelectorConstructorPipelineName]`으로 표시 이름을 지정할 수 있습니다.
- 기본값·선택적 매개변수는 `ParameterInfo.HasDefaultValue`/`DefaultValue`를 그대로 사용합니다.
- 생성자 인자는 "생성 직후 객체 필드에 값을 반영하는 방식"을 택했습니다 — 생성자 자신이 인자를
  자신의 직렬화 필드에 대입하는 일반적인 C# 관례에 의존하며, 별도의 생성 설정 직렬화는 두지
  않습니다.
- **사용자 Unity Editor 실행 검증 통과**: 오버로드 드롭다운, 매개변수 필드(int/bool/double/Color/
  Vector2/Vector3/Vector4/enum/`UnityEngine.Object`), 선택적 기본값, 생성자 예외 표시, Undo(생성
  취소 시 이전 상태 복원), 다중 오브젝트 선택 시 각 대상에 독립된 인스턴스가 생성되는지를 Sample의
  `StunAttributeEffect`/`ElementalAttributeEffect`로 확인했습니다.
- 생성자 호출과 managed-reference 적용을 `ConstructorSelectionService`/
  `ManagedReferenceAssignmentService`로 분리했습니다. 다중 선택 대상의 인스턴스를 모두 먼저 생성한
  뒤 적용하므로 중간 생성 실패 시 어떤 대상도 바뀌지 않으며, 대상마다 독립 인스턴스를 사용합니다.
- 팝업에 타입 제목·단일 생성자 시그니처·높이 제한 스크롤·생성자 예외 HelpBox를 추가했습니다.
  매개변수 없는 생성자의 예외는 대화상자로 표시합니다.
- EditMode 테스트(`SerializeReferenceConstructorTests`, 11개)를 추가해 오버로드 입력과 선택적
  기본값, 파이프라인 후보·표시 이름과 Object/Enum/Vector 기본값, 생성 전 불변, 대상별 독립
  인스턴스, 중간 실패 시 전체 불변, Undo, 생성자 예외 메시지를 검증합니다. 사용자가 Unity Test
  Runner에서 11개 전체 PASS를 확인했습니다.
