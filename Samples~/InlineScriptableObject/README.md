# InlineScriptableObject 예제

`[InlineScriptableObject]`를 붙인 ScriptableObject 참조 필드가 참조한 자산의 Inspector를
컴포넌트 Inspector 안에서 펼쳐 편집하는지 확인하는 예제입니다. 이 Sample은 Attributes 패키지만
사용합니다.

## 구성

- `InlineScriptableObjectSampleSettings`: 예제 전용 ScriptableObject(`Label`, `Speed`, `Count`, `Tint`)
- `SharedSampleSettings.asset`, `AlternateSampleSettings.asset`: 서로 다른 값을 가진 예제 자산
- `InlineScriptableObjectSample`: `[SerializeField, InlineScriptableObject]` 필드 하나를 가진 컴포넌트
- `InlineScriptableObjectSample.unity`: 아래 GameObject 4개를 배치한 Scene

| GameObject | `Settings` 참조 |
| --- | --- |
| `Shared Settings A` | `SharedSampleSettings` |
| `Shared Settings B` | `SharedSampleSettings` |
| `Alternate Settings` | `AlternateSampleSettings` |
| `No Settings` | 없음 |

## 확인 절차

`InlineScriptableObjectSample.unity`를 열고 다음을 확인합니다.

1. **참조 있음**: `Shared Settings A`를 선택합니다. `Settings` 참조 필드 아래에 접힌
   `Asset Properties` Foldout이 표시되고, 펼치면 `Label`, `Speed`, `Count`, `Tint`가 표시됩니다.
2. **참조 없음**: `No Settings`를 선택합니다. 참조 필드만 표시되고 `Asset Properties`는 표시되지 않습니다.
   이 상태에서 참조 필드에 `AlternateSampleSettings`를 지정하면 Foldout이 나타나고, 다시 `None`으로
   지우면 숨겨집니다. 확인 후 Undo로 원래 상태(참조 없음)로 되돌립니다.
3. **공유 자산 값 변경**: `Shared Settings A`에서 `Asset Properties`를 펼쳐 `Speed`나 `Label`을
   바꿉니다. `Shared Settings B`를 선택해 펼치면 같은 값이 보이고, Project 창에서
   `SharedSampleSettings.asset`을 선택해도 같은 값입니다. 값은 컴포넌트 사본이 아닌 공유 자산에
   저장됩니다. Undo로 변경을 되돌릴 수 있는지도 확인합니다.
4. **다른 자산은 독립**: `Alternate Settings`를 선택해 펼치면 3번에서 바꾼 값과 무관한
   `AlternateSampleSettings`의 값이 표시됩니다.
5. **다중 선택, 같은 참조**: `Shared Settings A`와 `Shared Settings B`를 함께 선택하면
   `Asset Properties`가 표시됩니다.
6. **다중 선택, 다른 참조**: `Shared Settings A`와 `Alternate Settings`(또는 `No Settings`)를 함께
   선택하면 참조 필드는 혼합 값으로 표시되고 `Asset Properties`는 숨겨집니다.
7. **참조 교체**: `Shared Settings A`의 참조를 `AlternateSampleSettings`로 바꾸고 `Asset Properties`를
   펼치면 교체한 자산의 값으로 갱신됩니다. 확인 후 Undo로 원래 참조로 되돌립니다.

예제 자산의 값을 바꾼 채 저장했다면 Sample을 다시 Import해 초기 값으로 되돌릴 수 있습니다.
