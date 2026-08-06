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

### P3-02 — API·문서 정리 (완료)

- 공개 Attribute별 지원 필드 타입, 호출 시점, Undo/Prefab 동작을 한·영 README 표로 정리합니다.
- 기존 0.x API 제거 목록과 마이그레이션 예시를 CHANGELOG에 보강합니다.
- `InspectorButton` 메서드 인자 지원 범위와 향후 입력 UI 계획을 명시합니다.

### P3-03 — Source Generator 타당성 재평가 (추후 작업)

- 실제 프로젝트에서 Reflection 비용과 Inspector 갱신 빈도를 측정합니다.
- 측정 결과가 유의미할 때만 생성 코드 도입 범위와 Unity Editor 호환 전략을 설계합니다.
