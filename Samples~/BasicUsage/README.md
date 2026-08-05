# Attributes 기본 예제

`AttributesSample`은 범용 Attribute의 Inspector 표시와 콜백을 확인하는 예제입니다.

`InspectorButtonSample` Scene에서는 다음 동작을 확인합니다.

- 값 변경 콜백이 Undo 변경 스트림을 통해 호출되는지 확인합니다.
- 여러 GameObject를 동시에 선택했을 때 Inspector 버튼이 모든 대상에 호출되는지 확인합니다.

`ObjectAssignmentSample` Scene에서는 `Object Assignment Sample` GameObject를 선택해
다음 동작을 확인합니다.

- `GetOrAddComponent`가 누락된 `BoxCollider`를 추가하고 필드에 연결하는지 확인합니다.
- Undo 한 번으로 컴포넌트 추가와 필드 연결을 되돌릴 수 있는지 확인합니다.
- `HierarchyObjectPicker` 드롭다운에서 `Candidate A`, `Candidate B`를 선택해
  GameObject 및 Transform 필드에 올바르게 할당되는지 확인합니다.
