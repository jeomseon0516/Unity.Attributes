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
- 메서드 실행: `InspectorButton`, `InvokeOnInspectorChange`
- 각 Attribute의 PropertyDrawer, Inspector UI 및 메서드 실행 구현

`InvokeOnInspectorChange`는 Inspector나 Editor 도구가 Undo를 통해 직렬화 필드를
변경했을 때 매개변수 없는 메서드를 지연 호출합니다. 런타임 값 변경 알림 용도가 아닙니다.

## 패키지 경계

- 이 패키지는 특정 기능 도메인에 의존하지 않는 Inspector Attribute의 선언과 Editor 구현을 소유합니다.
- 공통 Editor API는 `com.jeomseon.unity.editor-toolkit`을 단방향으로 참조해 재사용합니다.
- 기능 전용 Attribute는 해당 기능 패키지가 소유합니다. 예를 들어 `LocalizedStringAttribute`는
  `com.jeomseon.unity.localization`, `ResetOnPoolReleaseAttribute`는
  `com.jeomseon.unity.game-object-pooling`에서 제공합니다.
- Runtime asmdef는 Editor 어셈블리를 참조하지 않으며 Editor 의존성은 Editor asmdef에만 제한됩니다.

## 라이선스

[MIT License](./LICENSE.md)
