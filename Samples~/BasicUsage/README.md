# Attributes 기본 예제

`AttributesSample`은 범용 Attribute의 Inspector 표시와 콜백을 확인하는 예제입니다.

## UI Toolkit 및 Reload 검증

`AttributesSample` Scene에서 오브젝트를 선택해 다음 항목을 한 화면에서 확인합니다.

- `InfoBox`: 안내 메시지가 표시되고 좁은 Inspector 폭에서 여러 줄로 줄바꿈됩니다.
- `MaxValue`: 최대값을 초과해 입력해도 지정한 상한으로 제한됩니다.
- `ReadOnly`: 필드가 비활성 상태로 표시되어 값을 편집할 수 없습니다.

Injection 수명 검증은 다음 매트릭스로 진행합니다.

| 조건 | 기대 결과 |
| --- | --- |
| Injection 활성 | Inspector 하단 버튼 1개 표시 및 호출 |
| Injection 비활성 | Inspector Header fallback 버튼 1개 표시 및 호출 |
| Assembly Reload 반복 | 버튼 중복·누수 없이 1개 유지 |
| Domain Reload 켬/끔 | 재활성화 후 동일한 표시와 호출 |

`InspectorButtonSample` Scene에서는 다음 동작을 확인합니다.

- 값 변경 콜백이 Undo 변경 스트림을 통해 호출되는지 확인합니다.
- 여러 GameObject를 동시에 선택했을 때 Inspector 버튼이 모든 대상에 호출되는지 확인합니다.

`ObjectAssignmentSample` Scene에서는 `Object Assignment Sample` GameObject를 선택해
다음 동작을 확인합니다.

- `GetOrAddComponent`가 누락된 `BoxCollider`를 추가하고 필드에 연결하는지 확인합니다.
- Undo 한 번으로 컴포넌트 추가와 필드 연결을 되돌릴 수 있는지 확인합니다.
- `HierarchyObjectPicker` 드롭다운에서 `Candidate A`, `Candidate B`를 선택해
  GameObject 및 Transform 필드에 올바르게 할당되는지 확인합니다.

`SerializeReferenceSelectorSample` Scene에서는 `Serialize Reference Selector Sample` GameObject를
선택해 다음 동작을 확인합니다.

- `Selected Effect` 드롭다운에서 `DamageAttributeEffect`, `HealAttributeEffect`, `(None)`을 선택합니다.
- `Effect Sequence`의 Size와 각 원소의 구체 타입을 변경하고 서로 다른 하위 필드를 편집합니다.
- Scene을 저장하고 다시 열어 선택한 구체 타입과 값이 유지되는지 확인합니다.
- Play Mode에서 타입과 값을 변경한 뒤 Play Mode를 종료해, 진입 전 Edit Mode 값으로 복원되는지
  확인합니다.
- 매개변수 생성자 지원 확인: `Selected Effect` 드롭다운에서 `StunAttributeEffect`를 선택합니다.
  생성자가 여러 개(매개변수 없음 / 단순 인자 / Object·Enum·Vector 인자)라 즉시 생성되지 않고,
  타입 버튼 아래에 AdvancedDropdown과 같은 방식의 뜬 팝업 창("생성자 선택")이 나타나는지
  확인합니다. 이 시점에는 `Selected Effect` 필드 자체는 아직 아무것도 바뀌지 않아야 합니다(이전
  값 또는 `(None)` 그대로).
  - 팝업의 오버로드 드롭다운에서 `StunAttributeEffect(float duration, string description)`을
    고르면 팝업 안에 `duration`(float)·`description`(string) 입력 필드와 "생성"/"취소" 버튼이
    나타나는지 확인합니다.
  - 값을 입력하고 "생성"을 누르면 팝업이 닫히고, 그 값 그대로 `Duration`·`Description` 필드를 가진
    인스턴스가 `Selected Effect`에 대입되는지, Scene 저장·재오픈 후에도 값이 유지되는지 확인합니다.
  - `StunAttributeEffect(GameObject, StunDirectionMode, Vector3, float, string)` 오버로드를 선택해
    Object·Enum·Vector 필드가 올바르게 표시되는지 확인합니다. `duration`은 `1`, `description`은
    `생성자 기본 설명`으로 초기화되어야 합니다. 더 많은 매개변수를 가진 소비자 타입에서는 팝업이
    최대 높이에서 멈추고 내부 스크롤로 생성/취소 버튼까지 이동할 수 있어야 합니다.
  - 생성자가 예외를 던지는 소비자 타입을 사용할 경우 팝업이 닫히거나 기존 값이 바뀌지 않고, 팝업
    아래쪽에 예외 종류와 메시지가 표시되어야 합니다.
  - 다시 타입을 바꿔 팝업을 띄운 뒤 "취소"를 누르거나 팝업 밖을 클릭해 닫으면, `Selected Effect`
    필드가 이전 상태(`(None)` 또는 이전 타입) 그대로 남아 있는지(즉, 아무것도 대입되지 않는지)
    확인합니다.
  - `Effect Sequence` 리스트 원소에서도 동일하게 `StunAttributeEffect`를 선택해 팝업이 리스트 원소
    단위로 독립적으로 동작하는지 확인합니다.
  - Sample 컴포넌트가 붙은 GameObject 두 개를 동시에 선택해 같은 생성자를 실행하고, 생성 후 한쪽의
    값을 변경해도 다른 쪽 값이 바뀌지 않는지 확인합니다. Undo 한 번으로 두 대상 모두 이전 값으로
    복원되어야 합니다.
- 나머지 기본 제공 파이프라인 확인: `Selected Effect` 드롭다운에서 `ElementalAttributeEffect`를
  선택합니다. `StunAttributeEffect`가 다루지 않는 `int`/`bool`/`double`/`Color`/`Vector2`/`Vector4`
  파이프라인과 생성자 예외 표시를 이 타입으로 확인합니다.
  - `ElementalAttributeEffect(int stacks, bool isCritical)` 오버로드를 선택해 정수·불리언 필드가
    올바르게 표시·입력되는지 확인합니다.
  - `ElementalAttributeEffect(double multiplier, Color effectColor)` 오버로드를 선택해 double 필드와
    Color 필드(컬러 피커)가 올바르게 표시·입력되는지 확인합니다.
  - `ElementalAttributeEffect(Vector2 areaSize, Vector4 falloffCurve, double multiplier = 1.0, int stacks = 1)`
    오버로드를 선택해 Vector2·Vector4 필드가 표시되는지, `multiplier`는 `1`, `stacks`는 `1`로
    초기화되는지 확인합니다.
  - `ElementalAttributeEffect(string invalidConfiguration)` 오버로드를 선택하고 아무 문자열이나 입력한
    뒤 "생성"을 누르면, 팝업이 닫히지 않고 팝업 아래쪽에 `InvalidOperationException`과 예외 메시지가
    표시되는지, `Selected Effect` 필드는 그대로 유지되는지 확인합니다.
