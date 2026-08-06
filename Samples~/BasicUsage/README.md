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
