# Attributes 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 작업 순서

1. **P1-01 — Attribute 소유권과 의존성 경계 확정 (완료)**
   - 범용 Attribute 선언과 Editor 구현을 이 패키지에 함께 유지합니다.
   - 기능 전용 Attribute 선언과 Editor 구현은 해당 기능 패키지가 소유합니다.
   - Runtime 어셈블리는 Editor 의존성을 갖지 않고, Editor asmdef만 EditorToolkit 공통 API를 단방향으로 참조합니다.
   - `LocalizedStringAttribute`와 `ResetOnPoolReleaseAttribute`가 각 기능 패키지에 있음을 확인했습니다.
   - EditorToolkit의 Attribute 구현과 샘플을 이 패키지로 이동하고 역방향 의존성을 제거했습니다.
2. **P1-02 — 공개 API 이름 정리 (완료)**
   - `OnwMin`은 Unity `MinAttribute`로 대체하고 `OnwMax`는 `MaxValueAttribute`로 변경했습니다.
   - 사용 중단된 비제네릭 TreeView API를 Unity 6 `TreeView<int>` API로 교체했습니다.
   - `OnChangedValueByValue`를 제거하고 `OnChangedValueForMethod`를 `InvokeOnInspectorChange`로 변경했습니다.
   - `InfoBoxType`, `Vector2Slider`, `GetOrAddComponent`, `HierarchyObjectPicker`로 공개 API 이름을 정리했습니다.
   - Unity `InspectorNameAttribute`와 중복되는 `DisplayAsAttribute`를 제거했습니다.
3. **P2-01 — Attribute 계약 테스트 (완료)**
   - `AttributeUsage`, 상속, 다중 적용 및 Conditional 동작을 테스트합니다.
   - `MaxValueAttribute` 상한 처리와 `InvokeOnInspectorChange` 호출 중복 방지를 테스트합니다.
4. **P3-01 — Source Generator 가능성 검토**
   - Reflection 기반 처리 비용이 큰 기능만 생성 코드로 대체할 가치가 있는지 평가합니다.
