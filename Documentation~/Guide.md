# Attributes: 사용과 설계

[English](Guide.en.md) · [설치](../README.md)

## 사용 범위

반복되는 Inspector 표시·오브젝트 연결·메서드 호출·다형성 값 생성을 Attribute 선언으로
구성합니다. 게임 도메인의 실행 로직은 포함하지 않습니다. Attribute 선언은 Runtime 어셈블리에,
Drawer와 타입 탐색·반사 호출·Undo 처리는 Editor 어셈블리에 둡니다.
Localization이나 Pooling 전용 Attribute는 해당 기능 패키지가 소유합니다.

| 작업 | API | 주의점 |
| --- | --- | --- |
| 안내·미리보기·읽기 전용 표시 | `InfoBox`, `SpritePreview`, `ReadOnly` | Inspector 표현이며 런타임 접근 제어가 아님 |
| 수치 입력 | `MaxValue`, `Vector2Slider` | 최소값 제한만 필요하면 Unity `Min` 사용 |
| 참조 연결 | `GetOrAddComponent`, `HierarchyObjectPicker` | 실제 Scene·Prefab 참조와 Undo 결과를 확인 |
| 버튼 실행 | `InspectorButton` | 편집 도구 동작과 런타임 초기화를 구분 |
| 편집 변경에 응답 | `InvokeOnInspectorChange` | Undo 변경 스트림 기반, 매개변수 없는 메서드 |
| 다형성 참조 생성 | `SerializeReferenceSelector` | Unity `[SerializeReference]`와 함께 사용 |

`ReadOnly`는 코드에서 값을 수정하는 것을 막지 않습니다. `InvokeOnInspectorChange`는
런타임 setter나 C# event를 대신하지 않습니다. Inspector Injection 설정과 Header fallback은
같은 버튼을 중복 표시하지 않도록 선택되는 경로입니다.

## 실제 예제

[AttributesSample.cs](../Samples~/BasicUsage/AttributesSample.cs)는 `InfoBox`, `ReadOnly`,
`MaxValue`, `InspectorButton`, `InvokeOnInspectorChange`를 함께 보여줍니다.
사용 시 Attribute namespace는 `Jeomseon.Unity.Attributes`입니다.

```csharp
[InfoBox("10 이하의 값으로 편집합니다.")]
[MaxValue(10)]
[UnityEngine.SerializeField] private int count;

[InvokeOnInspectorChange(nameof(count))]
private void OnCountEdited() => UnityEngine.Debug.Log(count);
```

위 Attribute를 사용하는 소비 스크립트는 해당 namespace를 import해야 합니다. 완성된 실행·편집
예제는 [Basic Usage](../Samples~/BasicUsage/README.md)의 Scene을 Import해 확인하세요.

## SerializeReference 생성 흐름

1. 직렬화 필드의 기준 타입을 해석하고 생성 가능한 구체 타입을 찾습니다.
2. 매개변수 없는 생성만 필요한 경우 값을 직접 생성합니다. 인자가 필요하면 생성자 팝업에서 선택합니다.
3. 지원되는 public 생성자마다 입력 UI와 기본 인자를 준비합니다.
4. 생성 성공 시 대상마다 새 인스턴스를 만들고 Undo 가능한 할당을 수행합니다.
5. 생성 실패 또는 취소 시 이전 값을 유지합니다. `(None)`은 명시적으로 null을 할당합니다.

입력 파이프라인은 `int`, `float`, `double`, `bool`, `string`, enum, Unity Object, Color,
Vector2/3/4를 지원합니다. 지원하지 않는 매개변수가 있는 생성자는 후보에 포함되지 않습니다.
Editor 전용 `ISerializeReferenceSelectorConstructorParameterPipeline` 구현을 TypeCache로 검색하므로
사용자 인자 타입의 입력기를 추가할 수 있습니다. 파이프라인 구현에는 public 매개변수 없는 생성자가
필요합니다. 지원 가능 여부와 기본값, 필드 그리기를 구현하며 선택적 이름 Attribute로 표시명을 지정합니다.

## 코드와 검증 경로

- [Attribute 선언](../Runtime): 런타임/에디터 경계와 사용자 문법
- [Selector Drawer](../Editor/SerializeReferenceSelectorDrawer.cs): 타입 선택과 팝업 연결
- [생성자 선택](../Editor/ConstructorSelectionService.cs): 오버로드·기본값·예외
- [입력 파이프라인](../Editor/ConstructorPipelines): 지원 타입과 확장 계약
- [테스트](../Tests/Editor): 생성자, 다중 선택, 직렬화 복원과 Editor 구현 회귀

Scene 저장·재오픈, 다중 선택의 독립 인스턴스, 생성자 예외, 취소, Undo,
Assembly/Domain Reload에 따른 중복 버튼 여부를 [샘플 절차](../Samples~/BasicUsage/README.md)로 확인합니다.
