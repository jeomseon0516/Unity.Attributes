# Jeomseon Unity Attributes

한국어 | [English](./README.en.md)

범용 Inspector Attribute와 해당 Editor 구현을 함께 제공하는 개발 편의 패키지입니다.

## 요구 사항

- Unity 6000.5.7f1 이상

## 설치

```json
{
  "dependencies": {
    "com.jeomseon.unity.attributes": "0.3.0"
  }
}
```

## 포함 기능

- 표시·편집: `InfoBox`, `ReadOnly`, `SpritePreview`, `Vector2Slider`
- 값 제한: `MaxValue` (`Min`은 Unity 기본 API 사용)
- 오브젝트 연결: `GetOrAddComponent`, `HierarchyObjectPicker`
- 다형성 참조: `SerializeReferenceSelector` (`SerializeReference` 단일 필드·리스트의 구체 타입 선택)
- 메서드 실행: `InspectorButton`, `InvokeOnInspectorChange`
- 각 Attribute의 PropertyDrawer, Inspector UI 및 메서드 실행 구현

## Sample 검증

`Samples~/BasicUsage/AttributesSample` Scene은 UI Toolkit Inspector에서 `InfoBox`, `MaxValue`,
`ReadOnly`를 한 화면에 확인할 수 있도록 구성되어 있습니다. Inspector 폭을 좁혀 InfoBox가
여러 줄로 표시되고 텍스트가 잘리지 않는지 확인합니다.

Injection 회귀 확인은 다음 순서로 진행합니다.

1. `Project Settings > Jeomseon > Attributes`에서 Injection을 활성화하고 InspectorButton이 하나만 표시되는지 확인합니다.
2. Injection을 비활성화하고 Inspector Header fallback 버튼이 하나만 표시되는지 확인합니다.
3. Assembly Reload와 Domain Reload를 각각 활성화/비활성화한 뒤 버튼 중복, 누수, 재활성화 실패가 없는지 확인합니다.

`InvokeOnInspectorChange`는 Inspector나 Editor 도구가 Undo를 통해 직렬화 필드를
변경했을 때 매개변수 없는 메서드를 지연 호출합니다. 런타임 값 변경 알림 용도가 아닙니다.

`SerializeReferenceSelector`는 `[SerializeReference]`와 함께 사용합니다. 선택 가능한 구체 타입에는
public 매개변수 없는 생성자가 필요하며, `(None)`을 선택하면 참조 또는 리스트 원소가 `null`이 됩니다.
매개변수 생성자 선택과 인자 입력 UI는 아직 지원하지 않습니다. 현재는 소비 타입에 매개변수 없는
생성자를 추가하고 생성 후 표시되는 직렬화 필드를 Inspector에서 편집합니다.

## 패키지 경계

- 이 패키지는 특정 기능 도메인에 의존하지 않는 Inspector Attribute의 선언과 Editor 구현을 소유합니다.
- 공통 Editor API는 `com.jeomseon.unity.editor-toolkit`을 단방향으로 참조해 재사용합니다.
- 기능 전용 Attribute는 해당 기능 패키지가 소유합니다. 예를 들어 `LocalizedStringAttribute`는
  `com.jeomseon.unity.localization`, `ResetOnPoolReleaseAttribute`는
  `com.jeomseon.unity.game-object-pooling`에서 제공합니다.
- Runtime asmdef는 Editor 어셈블리를 참조하지 않으며 Editor 의존성은 Editor asmdef에만 제한됩니다.

## 라이선스

[MIT License](./LICENSE.md)
