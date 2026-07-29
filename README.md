# Jeomseon Unity Attributes

한국어 | [English](./README.en.md)

Unity Inspector 확장에 사용하는 범용 Attribute 선언을 제공하는 경량 패키지입니다.

## 요구 사항

- Unity 2022.3 이상

## 설치

```json
{
  "dependencies": {
    "com.jeomseon.unity.attributes": "0.2.0"
  }
}
```

## 포함 기능

- 필드 표시 및 편집 제어 Attribute
- Inspector 버튼용 메서드 Attribute
- 값 변경 감지용 메서드 Attribute
- 컴포넌트 자동 연결 지원 Attribute

PropertyDrawer와 메서드 메타데이터 UI는 `com.jeomseon.unity.editor-toolkit`에서 제공합니다.
Localization 전용 `LocalizedStringAttribute`는 `com.jeomseon.unity.localization`에서 제공합니다.

## 라이선스

[MIT License](./LICENSE.md)
